using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float CollisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;
    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    private void FixedUpdate()
    {
        if(canMove) {
            if(movementInput != Vector2.zero)
            {   
                bool success = TryMove(movementInput);

                if(!success) {
                    success = TryMove(new Vector2(movementInput.x, 0));
                    if(!success ) {
                        success = TryMove(new Vector2(0, movementInput.y));
                    }
                }
                animator.SetBool("isMoving", success);

            //set direction of player to movement direction
            if(movementInput.x > 0) {
                spriteRenderer.flipX = false;
            } else if(movementInput.x < 0) {
                spriteRenderer.flipX = true;
            }
             if (movementInput.y > 0 && movementInput.x == 0)
            {
                animator.SetBool("isMovingUp", true);
                animator.SetBool("isMovingDown", false);
            }
            else if (movementInput.y < 0 && movementInput.x == 0)
            {
                animator.SetBool("isMovingUp", false);
                animator.SetBool("isMovingDown", true);
            } else {
                animator.SetBool("isMovingUp", false);
                animator.SetBool("isMovingDown", false);
            }
            } else {
                animator.SetBool("isMoving", false);
                animator.SetBool("isMovingUp", false);
                animator.SetBool("isMovingDown", false);
            }
        }
    }

    private bool TryMove(Vector2 direction) 
    {
        if(direction != Vector2.zero) {
            //Sjekker om det er en kollisjon før man gjør et move
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + CollisionOffset);
            if(count == 0) {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    void OnMove(InputValue value)   {
        movementInput = value.Get<Vector2>();
    }

    void OnFire() {
        animator.SetTrigger("swordAttack");
    }

    public void SwordAttack() {
        LockMovement();
        if(animator.GetBool("isMovingUp")) {
            swordAttack.AttackUp();
        } else if(animator.GetBool("isMovingDown")) {
            swordAttack.AttackDown();
        } else if(spriteRenderer.flipX == true) {
            swordAttack.AttackLeft();
        } else {
            swordAttack.AttackRight();
        }
    }

    public void StopSwordAttack() {
        UnlockMovement();
        swordAttack.StopAttack();
    }
    public void LockMovement() {
        canMove = false;
        
    }

    public void UnlockMovement() {
        canMove = true;
        
    }
}
