using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DamageableCharacter : MonoBehaviour, IDamageable
{
    Animator animator;
    public Rigidbody2D rb;
    public Collider2D physicsCollider;

    public float Health
    {
        set
        {
            if (value < health)
            {
                animator.SetTrigger("Damaged");
            }
            health = value;
            if (health <= 0)
            {
                targetable = false;
                animator.SetTrigger("Defeated");
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
    private float health = 10;
    public bool targetable = true;
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCollider = GetComponent<Collider2D>();

    }


    private void Defeated()
    {
        animator.SetTrigger("Defeated");
    }


    public void RemoveSelf()
    {
        Destroy(gameObject);
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        Health -= damage;
        rb.AddForce(knockback);
        animator.SetTrigger("Damaged");
    }

    public void OnHit(float damage)
    {
        Health -= damage;
    }

}
