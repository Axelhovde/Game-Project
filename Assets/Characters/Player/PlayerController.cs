using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 150f;
    public float maxSpeed = 1.7f;
    public float idleFriction = 0.9f;
    /*     public int maxHealth = 100;
        public int maxStamina = 100; */
    /*     public int currentHealth; */
    public SwordAttack swordAttack;
    public PlayerStats playerStats;
    Vector2 moveInput = Vector2.zero;
    SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    Animator animator;
    bool canMove = true;
    private bool isDodging = false;
    private Vector2 dodgeDirection;
    private bool dodgeSet = false;
    private bool inStairs = false;
    public Inventory inventory;
    Vector2 lastMoveInput = Vector2.zero;
    private bool PerformingAction = false;
    private bool pendingAttack = false;
    private Vector2 pendingDirection;

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
                if (!PerformingAction)
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
                if (moveInput.x > 0 && !PerformingAction)
                {
                    spriteRenderer.flipX = false;
                }
                else if (moveInput.x < 0 && !PerformingAction)
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
        lastMoveInput = value.Get<Vector2>();

        if (inStairs && lastMoveInput.x != 0)
        {
            moveInput = new Vector2(lastMoveInput.x, lastMoveInput.y - lastMoveInput.x).normalized;
        }
        else
        {
            moveInput = lastMoveInput;
        }
    }

    void OnFire()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 direction = (worldMousePosition - (Vector2)transform.position).normalized;
        if (PerformingAction)
        {
            pendingAttack = true;
            pendingDirection = direction;

        }
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                if (!PerformingAction) spriteRenderer.flipX = false;
                animator.SetTrigger("attackRightLeft");

            }
            else
            {
                if (!PerformingAction) spriteRenderer.flipX = true;
                animator.SetTrigger("attackRightLeft");

            }
        }
        else
        {
            if (direction.y > 0)
            {
                animator.SetTrigger("attackUp");
            }
            else
            {
                animator.SetTrigger("attackDown");
            }
        }
        IsPerformingAction(true);
    }

    public void AttackDirection()
    {
        if (Mathf.Abs(pendingDirection.x) > Mathf.Abs(pendingDirection.y))
        {
            if (pendingDirection.x > 0)
            {
                spriteRenderer.flipX = false;

            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
        pendingAttack = false;
        pendingDirection = Vector2.zero;
    }

    public void SwordAttack()
    {
        LockMovement();
        if (animator.GetAnimatorTransitionInfo(0).IsName("player_attackUp"))
        {
            swordAttack.AttackUp();
        }
        else if (animator.GetAnimatorTransitionInfo(0).IsName("player_AttackDownWindUp"))
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

    public void StopAction()
    {
        print(pendingAttack);
        if (!pendingAttack)
        {
            IsPerformingAction(false);
            UpdateAnimatorParameters();
        }
        pendingAttack = false;
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
    public void IsPerformingAction(bool isPerformingAction)
    {
        PerformingAction = isPerformingAction;
    }

    void OnBlock()
    {
        animator.SetBool("Block", true);
    }
    void OnDodge()
    {
        if (!isDodging && playerStats.Stamina >= 20)
        {
            isDodging = true;
            LockMovement();
            playerStats.UpdateStamina(-20f);
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
                print(dodgeDirection);
            }
            else
            {
                float inputX = Input.GetAxisRaw("Horizontal");
                float inputY = Input.GetAxisRaw("Vertical");
                //dodgeDirection is the input x and y values rounded to 1 or 0, and then normalized
                dodgeDirection = new Vector2(Mathf.Round(inputX), Mathf.Round(inputY)).normalized;
                print("optional direction: " + dodgeDirection);
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

    public void SetInStairs(bool inStairs)
    {
        this.inStairs = inStairs;
        UpdateMoveInput();
    }

    private void UpdateMoveInput()
    {
        if (inStairs && lastMoveInput.x != 0)
        {
            moveInput = new Vector2(lastMoveInput.x, lastMoveInput.y - lastMoveInput.x).normalized;
        }
        else
        {
            moveInput = lastMoveInput;
        }
    }

    public void StartBlinkEffect()
    {
        playerStats.SetInvincibility(true);
        StartCoroutine(BlinkEffect());
    }
    private IEnumerator BlinkEffect()
    {
        // Blink red initially
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
        // Blink transparency between 0.7 and 1 for a second
        float duration = 1.0f;
        float blinkInterval = 0.2f;
        float elapsed = 0f;
        bool isTransparent = false;

        while (elapsed < duration)
        {
            elapsed += blinkInterval;
            if (isTransparent)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            }
            else
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.7f);
            }

            isTransparent = !isTransparent;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Reset to original color (assuming original color is fully opaque white)
        spriteRenderer.color = Color.white;
        playerStats.SetInvincibility(false);
    }


    //if player collide with something, check if it has an item tag, and if it has, add it to the inventory
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Item")
        {
            Debug.Log("Item picked up");
            inventory.AddItem(other.GetComponent<IInventoryItem>());
        }
    }
}
