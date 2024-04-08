using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator animator;
    
    public float detectionRadius = 0.7f;
    public float moveSpeed = 0.5f;
    public float CollisionOffset = 0.01f;
    private Transform player;
    private Rigidbody2D rb;
     public ContactFilter2D movementFilter;
     List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public bool hit = false;
    public bool canMove = true;

    public float Health {
        set {
            health = value;
            if(health <= 0) {
                Defeated();
            } else {
                Damaged();
            }
        }
        get {
            return health;
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

    }
    
    private void FixedUpdate()
    {
        Vector2 playerFeetPosition = player.position + new Vector3(0, -0.16f, 0);
        Vector2 directionToPlayer = (playerFeetPosition - (Vector2)transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius && !hit && canMove)
        {
            bool isMoving = TryMove(directionToPlayer);
            if(!isMoving) {
                isMoving = TryMove(new Vector2(directionToPlayer.x, 0));
                if(!isMoving) {
                    isMoving = TryMove(new Vector2(0, directionToPlayer.y));
                }
            }
            animator.SetBool("IsMoving", isMoving);
        }  else if (hit && canMove)
        {
            Vector2 directionFromPlayer = (playerFeetPosition - (Vector2)transform.position).normalized * -1;
            moveSpeed = 3f;
            bool isMoving = TryMove(directionFromPlayer);
            if(!isMoving) {
                isMoving = TryMove(new Vector2(directionFromPlayer.x, 0));
                if(!isMoving) {
                    isMoving = TryMove(new Vector2(0, directionFromPlayer.y));
            }
            }
        }
        
        else if (canMove) {
            animator.SetBool("IsMoving", false);
        }   
        
    }  

    private bool TryMove(Vector2 direction) 
    {
        if(direction != Vector2.zero) {
            // Check for a collision before moving
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

    private float health = 10;

    private void Damaged()
    {
        hit = true;
        animator.SetTrigger("Damaged");
        
    }

    public void EndDamaged()
    {
        hit = false;
        moveSpeed = 0.5f;
    }

    public void LockMovement()
    {
        /* canMove = false; */
        moveSpeed = 0.2f;
    }
    public void UnlockMovement()
    {
        /* canMove = true; */
        moveSpeed = 0.5f;
    }

    private void Defeated()
    {
        hit = true;
        animator.SetTrigger("Defeated");
    }

    public void RemoveEnemy()
    {
        Destroy(gameObject);
    }

}
