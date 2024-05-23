using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    Animator animator;
    public float damage = 18f;
    private Transform player;
    public Rigidbody2D rb;
    public CapsuleCollider2D physicsCollider;
    public ContactFilter2D movementFilter;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public bool hit = false;
    public bool canMove = true;

    public float Health
    {
        set
        {
            health = value;
            Damaged();
            if (health <= 0)
            {
                Defeated();
            }
        }
        get
        {
            return health;
        }
    }

    public bool Targetable
    {
        get
        { return targetable; }
        set
        {
            targetable = value;
            rb.simulated = value;
        }
    }
    public void SetTargetableFalse()
    {
        Targetable = false;

    }
    private float health = 10;
    public bool targetable = true;
    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            /*  Move(); */
        }
    }

    /* private void Move()
    {
        Vector2 direction = player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRadius, movementFilter.layerMask);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Player")
            {
                direction.Normalize();
                rb.MovePosition((Vector2)transform.position + direction * moveSpeed * Time.deltaTime);
            }
        }
    } */




    private void Damaged()
    {
        hit = true;
        animator.SetTrigger("Damaged");

    }


    public void LockMovement()
    {
        canMove = false;
    }
    public void UnlockMovement()
    {
        canMove = true;
    }

    private void Defeated()
    {
        hit = true;
        animator.SetTrigger("Defeated");
    }


    public void RemoveSelf()
    {
        Destroy(gameObject);
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        Debug.Log(knockback);
        Health -= damage;
        rb.AddForce(knockback);
    }

    public void OnHit(float damage)
    {
        Health -= damage;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageableObject = collision.collider.GetComponent<IDamageable>();
        //check if damageable object not "enemy" tag

        if (damageableObject != null && collision.gameObject.tag != "Enemy")
        {
            damageableObject.OnHit(damage);
        }
    }
}
