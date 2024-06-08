using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 150f;
    public float maxSpeed = 1.7f;
    public float idleFriction = 0.9f;
    /*     public int maxHealth = 100;
        public int maxStamina = 100; */
    /*     public int currentHealth; */
    public SwordAttack swordAttack;
    public PlayerStats playerHealth;
    Vector2 moveInput = Vector2.zero;
    SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    Animator animator;
    bool canMove = true;
    private bool isDodging = false;
    private Vector2 dodgeDirection;
    private bool dodgeSet = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Move();
        }
        if (!isDodging) { UpdateAnimatorParameters(); }
        else if (isDodging && !dodgeSet)
        {
            SetDodgeAnimatorParameters();
        }
    }

    void SetDodgeAnimatorParameters()
    {
        animator.SetBool("isMoving", true);
        animator.SetFloat("moveX", dodgeDirection.x);
        animator.SetFloat("moveY", dodgeDirection.y);
        animator.SetBool("isMoving", true);
        animator.SetTrigger("dodge");
        dodgeSet = true;
    }
    void UpdateAnimatorParameters()
    {
        animator.SetFloat("moveX", moveInput.x);
        animator.SetFloat("moveY", moveInput.y);
        if (moveInput == Vector2.zero)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
            //if moving in the x direction
            if (moveInput.x == 0)
            {
                animator.SetBool("isMovingX", false);
                spriteRenderer.flipX = false;
            }
            else
            {
                animator.SetBool("isMovingX", true);
            }
            //if moving in the y direction
            if (moveInput.y == 0)
            {
                animator.SetBool("isMovingY", false);
            }
            else
            {
                animator.SetBool("isMovingY", true);
            }
        }
    }
    private void Move()
    {
        if (isDodging)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity + (dodgeDirection * moveSpeed * Time.deltaTime), maxSpeed);
        }
        else
        {
            if (moveInput != Vector2.zero)
            {

                rb.velocity = Vector2.ClampMagnitude(rb.velocity + (moveInput * moveSpeed * Time.deltaTime), maxSpeed);
                if (moveInput.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
                else if (moveInput.x < 0)
                {
                    spriteRenderer.flipX = true;
                }

            }
            else
            {

            }
        }
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }


    void OnFire()
    {
        animator.SetTrigger("swordAttack");
    }

    public void SwordAttack()
    {
        LockMovement();
        if (!(animator.GetBool("isMovingX")) && animator.GetFloat("moveY") < 0)
        {
            swordAttack.AttackUp();
        }
        else if (!(animator.GetBool("isMovingX")) && animator.GetFloat("moveY") > 0)
        {
            swordAttack.AttackDown();
        }
        else if (spriteRenderer.flipX == true)
        {
            swordAttack.AttackLeftRight(false);
        }
        else
        {
            swordAttack.AttackLeftRight(true);
        }
    }

    public void StopSwordAttack()
    {
        UnlockMovement();
        swordAttack.StopAttack();
    }
    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }

    void OnDodge()
    {
        if (!isDodging && playerHealth.Stamina >= 20)
        {
            isDodging = true;
            LockMovement();
            playerHealth.UpdateStamina(-20f);
            maxSpeed = maxSpeed * 2;
            //if not walking currently, dodge in the direction the player is facing
            if (moveInput == Vector2.zero)
            {
                //if animator is in player_idleUpwards state, dodge upwards
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_idleUpwards"))
                {
                    dodgeDirection = new Vector2(0, 1);
                }
                //if animator is in player_idle state, dodge downwards
                else if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_idle"))
                {
                    dodgeDirection = new Vector2(0, -1);
                }
                else if (spriteRenderer.flipX == true)
                {
                    dodgeDirection = new Vector2(-1, 0);
                }
                else
                {
                    dodgeDirection = new Vector2(1, 0);
                }
            }
            else
            {
                float inputX = Input.GetAxisRaw("Horizontal");
                float inputY = Input.GetAxisRaw("Vertical");
                //dodgeDirection is the input x and y values rounded to 1 or 0, and then normalized
                dodgeDirection = new Vector2(Mathf.Round(inputX), Mathf.Round(inputY)).normalized;
            }
        }


    }

    public void dodge()
    {
        UnlockMovement();


    }

    public void resetSpeed()
    {
        maxSpeed = 1.7f;
    }

    public void dodgeEnd()
    {

        isDodging = false;
        dodgeSet = false;
    }

}
