using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    public SwordAttack swordAttackDown;
    public SwordAttack swordAttackUp;
    public PlayerStats playerStats;
    Vector2 moveInput = Vector2.zero;
    SpriteRenderer spriteRenderer;
    SpriteRenderer weaponSpriteRenderer;
    public Rigidbody2D rb;
    Animator playerAnimator;
    Animator weaponAnimator;
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
    /* public GameObject crosshair; */
    public GameObject arrowPrefab;


    //TODO: FIX SELECTED WEAPON
    private String selectedWeapon = "";
    private float chargeTime = 0f;
    private float maxChargeTime = 1.5f; // Maximum charge time for full power
    private bool isCharging = false;
    private Vector2 fireDirection;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        weaponAnimator = transform.GetChild(0).GetComponent<Animator>();
        weaponSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector2 direction = (worldMousePosition - (Vector2)transform.position).normalized;
            CheckMouseDirection(direction);
        }
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

    public void SetSelectedWeapon(String weapon)
    {
        selectedWeapon = weapon;
    }
    public String GetSelectedWeapon()
    {
        return selectedWeapon;
    }
    void SetDodgeAnimatorParameters()
    {
        playerAnimator.SetBool("isMoving", true);
        playerAnimator.SetFloat("moveX", dodgeDirection.x);
        playerAnimator.SetFloat("moveY", dodgeDirection.y);
        playerAnimator.SetBool("isMoving", true);
        playerAnimator.SetTrigger("dodge");
        dodgeSet = true;
    }
    void UpdateAnimatorParameters()
    {
        playerAnimator.SetFloat("moveX", moveInput.x);
        playerAnimator.SetFloat("moveY", moveInput.y);
        if (moveInput == Vector2.zero)
        {
            playerAnimator.SetBool("isMoving", false);
        }
        else
        {
            playerAnimator.SetBool("isMoving", true);
            //if moving in the x direction
            if (moveInput.x == 0)
            {
                playerAnimator.SetBool("isMovingX", false);
                if (!PerformingAction)
                    spriteRenderer.flipX = false;
            }
            else
            {
                playerAnimator.SetBool("isMovingX", true);
            }
            //if moving in the y direction
            if (moveInput.y == 0)
            {
                playerAnimator.SetBool("isMovingY", false);
            }
            else
            {
                playerAnimator.SetBool("isMovingY", true);
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

    void OnFire(InputValue value)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 direction = (worldMousePosition - (Vector2)transform.position).normalized;
        if (selectedWeapon == "Bow")
        {
            if (value.isPressed)
            {
                CheckMouseDirection(direction);
                StartCharging();
                IsPerformingAction(true);
            }
            else
            {
                ReleaseFire();
            }
        }
        else if (selectedWeapon == "Sword")
        {
            if (value.isPressed)
            {
                Attack(direction);

            }
        }

    }

    #region Archery
    void StartCharging()
    {
        chargeTime = 0f;
        isCharging = true;
        LockMovement();

        // Optionally, you could start a charging animation here.
        playerAnimator.SetBool("BowCharge", true);
        weaponAnimator.SetBool("BowCharge", true);
    }

    void ReleaseFire()
    {
        if (!isCharging) return;

        isCharging = false;
        UnlockMovement();
        IsPerformingAction(false);
        float chargeRatio = chargeTime / maxChargeTime;

        // Adjust speed or damage based on charge time
        float arrowSpeed = Mathf.Lerp(1f, 3f, chargeRatio); // Example: Adjusting speed based on charge
        int arrowDamage = (int)Mathf.Lerp(2f, 5f, chargeRatio); // Example: Adjusting damage based on charge
        print(arrowDamage);
        float arrowKnockback = Mathf.Lerp(50f, 100f, chargeRatio); // Example: Adjusting knockback based on charge
        float arrowLifetime = Mathf.Lerp(0.2f, 5f, chargeRatio); // Example: Adjusting lifetime based on charge
        FireArrow(fireDirection, arrowSpeed, arrowDamage, arrowKnockback, arrowLifetime);

        // Optionally, you could stop the charging animation here.
        playerAnimator.SetBool("BowCharge", false);
        weaponAnimator.SetBool("BowCharge", false);
    }

    public void FireArrow(Vector2 direction, float speed, int damage, float knockbackForce, float lifetime)
    {
        if (playerStats.Stamina >= 10)
        {
            playerStats.UpdateStamina(-10f);

            // Make the arrow spawn a bit higher on the y-axis
            Vector2 spawnPosition = new Vector2(transform.position.x, transform.position.y + 0.17f);
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            fireDirection = (worldMousePosition - spawnPosition).normalized;
            GameObject arrowInstance = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);

            // Set the direction and speed of the arrow
            Arrow arrow = arrowInstance.GetComponent<Arrow>();
            arrow.SetDirectionAndSpeed(fireDirection, speed, damage, knockbackForce, lifetime);
        }
    }
    #endregion

    public void CheckMouseDirection(Vector2 direction)
    {
        if (isCharging)
        {
            if (direction.x < 0)
            {
                weaponSpriteRenderer.flipX = true;
                spriteRenderer.flipX = true;
            }
            else
            {
                weaponSpriteRenderer.flipX = false;
                spriteRenderer.flipX = false;
            }
        }
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                if (!PerformingAction)
                {
                    spriteRenderer.flipX = false;
                }

                playerAnimator.SetInteger("Direction", 0);
                weaponAnimator.SetInteger("Direction", 0);
            }
            else
            {
                if (!PerformingAction)
                {
                    spriteRenderer.flipX = true;
                }
                playerAnimator.SetInteger("Direction", 0);
                weaponAnimator.SetInteger("Direction", 0);
            }
        }
        else
        {
            if (direction.y > 0)
            {
                playerAnimator.SetInteger("Direction", 1);
                weaponAnimator.SetInteger("Direction", 1);
            }
            else
            {
                playerAnimator.SetInteger("Direction", -1);
                weaponAnimator.SetInteger("Direction", -1);
            }
        }
    }

    public void Attack(Vector2 direction)
    {
        if (PerformingAction)
        {
            pendingAttack = true;
            pendingDirection = direction;

        }
        CheckMouseDirection(direction);
        if(spriteRenderer.flipX == true)
        {
            weaponSpriteRenderer.flipX = true;
        }
        else
        {
            weaponSpriteRenderer.flipX = false;
        }
        IsPerformingAction(true);
        playerAnimator.SetTrigger("Attack");
        weaponAnimator.SetTrigger("Attack");
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
        playerStats.UpdateStamina(-7f);
        LockMovement();
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("player_AttackUp") ||
        playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("player_AttackUp2"))
        {
            swordAttackUp.Attack();
        }
        else if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("player_AttackDown") ||
        playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("player_AttackDown2"))
        {
            swordAttackDown.Attack();
        }
        else if (spriteRenderer.flipX == true)
        {
            swordAttack.Attack(false);
        }
        else
        {
            swordAttack.Attack(true);
        }
    }

    public void StopAction()
    {
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
        swordAttackDown.StopAttack();
        swordAttackUp.StopAttack();
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
        playerAnimator.SetBool("Block", true);
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
                if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("player_idleUpwards"))
                {
                    dodgeDirection = new Vector2(0, 1);
                }
                //if animator is in player_idle state, dodge downwards
                else if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("player_idle"))
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
    public void AttackStaminaUse()
    {
        playerStats.UpdateStamina(-5f);
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
