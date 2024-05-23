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
    public PlayerHealth playerHealth;
    Vector2 moveInput = Vector2.zero;
    SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    Animator animator;
    bool canMove = true;
    private bool isDodging = false;
    private Vector2 dodgeDirection;
    /*     public HealthBar healthBar;
        public StaminaBar staminaBar; */

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        /*         playerHealth.Health = maxHealth;
                playerHealth.SetMaxHealth(maxHealth);
                playerHealth.SetMaxStamina(maxStamina); */
        /*         StartCoroutine(SetStaminaOverTime()); */
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Move();
        }
        if (!isDodging) { UpdateAnimatorParameters(); }
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

    /*     IEnumerator SetStaminaOverTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                if (animator.GetBool("isMoving"))
                {
                    staminaBar.UpdateStamina(1);
                }
                else
                {
                    staminaBar.UpdateStamina(1);
                }
            }
        } */

    /* void Die()
    {
        /* Destroy(gameObject); */
    /* print("You died");
 } */

    void OnFire()
    {
        animator.SetTrigger("swordAttack");
    }
    /* 
        void UseStamina()
        {
            staminaBar.UpdateStamina(-15);
        } */

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
        animator.SetTrigger("dodge");
        LockMovement();
        playerHealth.UpdateStamina(-15f);
    }

    public void dodge()
    {
        UnlockMovement();

        isDodging = true;
        maxSpeed = 3f;
        //if not walking currently, dodge in the direction the player is facing
        if (moveInput == Vector2.zero)
        {
            if (spriteRenderer.flipX == true)
            {
                dodgeDirection = new Vector2(-1, 0);
            }
            else
            {
                dodgeDirection = new Vector2(1, 0);
            }
        }
        else
            dodgeDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

    }

    public void resetSpeed()
    {
        maxSpeed = 1.7f;
        moveSpeed = 150f;
    }

    public void dodgeEnd()
    {

        isDodging = false;
    }
    /* void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            takeDamage(10);

        }
    } */

}
