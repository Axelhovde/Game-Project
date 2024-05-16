using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 0.8f;
    public float bumpForce = 10f;
    public int maxHealth = 100;
    public int maxStamina = 100;
    public int currentHealth;
    public float CollisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;
    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    bool canMove = true;
    private bool isDodging = false;
    private Vector2 dodgeDirection;
    public HealthBar healthBar;
    public StaminaBar staminaBar;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        staminaBar.SetMaxStamina(maxStamina);
        StartCoroutine(SetStaminaOverTime());
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            if (isDodging)
            {
                bool success = TryMove(dodgeDirection);
            }
            else
            {
                if (movementInput != Vector2.zero)
                {
                    bool success = TryMove(movementInput);

                    if (!success)
                    {
                        success = TryMove(new Vector2(movementInput.x, 0));
                        if (!success)
                        {
                            success = TryMove(new Vector2(0, movementInput.y));
                        }
                    }
                    animator.SetBool("isMoving", success);
                    //set direction of player to movement direction
                    if (movementInput.x > 0)
                    {
                        spriteRenderer.flipX = false;
                    }
                    else if (movementInput.x < 0)
                    {
                        spriteRenderer.flipX = true;
                    }
                    if (success)
                    {
                        if (movementInput.y > 0 && movementInput.x == 0)
                        {
                            animator.SetBool("isMovingUp", true);
                            animator.SetBool("isMovingDown", false);
                            spriteRenderer.flipX = false;
                        }
                        else if (movementInput.y < 0 && movementInput.x == 0)
                        {
                            animator.SetBool("isMovingUp", false);
                            animator.SetBool("isMovingDown", true);
                            spriteRenderer.flipX = false;
                        }
                        else
                        {
                            animator.SetBool("isMovingUp", false);
                            animator.SetBool("isMovingDown", false);
                        }
                    }
                    else
                    {
                        animator.SetBool("isMoving", false);
                        animator.SetBool("isMovingUp", false);
                        animator.SetBool("isMovingDown", false);
                    }
                }
                else
                {
                    animator.SetBool("isMoving", false);
                    animator.SetBool("isMovingUp", false);
                    animator.SetBool("isMovingDown", false);
                }
            }
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            //Sjekker om det er en kollisjon før man gjør et move
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + CollisionOffset);
            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Slime"))
        {
            Rigidbody2D slimeRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            if (slimeRigidbody != null)
            {
                // Calculate the direction from the player to the slime
                Vector2 direction = (collision.gameObject.transform.position - transform.position).normalized;

                // Apply a force to move the slime in the direction away from the player
                slimeRigidbody.AddForce(direction * bumpForce, ForceMode2D.Impulse);
            }
        }
    }
    IEnumerator SetStaminaOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (animator.GetBool("isMoving"))
            {
                staminaBar.UpdateStamina(1);
                print("Stamina: " + staminaBar.slider.value);
            }
            else
            {
                staminaBar.UpdateStamina(1);
                print("Stamina: " + staminaBar.slider.value);
            }
        }
    }

    void takeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        /* Destroy(gameObject); */
        print("You died");
    }

    void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    void OnFire()
    {
        animator.SetTrigger("swordAttack");
    }

    void UseStamina()
    {
        staminaBar.UpdateStamina(-15);
    }

    void OnDodge()
    {
        animator.SetTrigger("dodge");
        LockMovement();
        takeDamage(10);
        staminaBar.UpdateStamina(-15);
    }

    public void SwordAttack()
    {
        LockMovement();
        if (animator.GetBool("isMovingUp"))
        {
            swordAttack.AttackUp();
        }
        else if (animator.GetBool("isMovingDown"))
        {
            swordAttack.AttackDown();
        }
        else if (spriteRenderer.flipX == true)
        {
            swordAttack.AttackLeft();
        }
        else
        {
            swordAttack.AttackRight();
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

    public void dodge()
    {
        UnlockMovement();

        isDodging = true;
        moveSpeed = moveSpeed * 2f;
        //if not walking currently, dodge in the direction the player is facing
        if (movementInput == Vector2.zero)
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
        moveSpeed = 0.8f;
    }

    public void dodgeEnd()
    {

        isDodging = false;
        UnlockMovement();
    }
}
