using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices.WindowsRuntime;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Slime : MonoBehaviour
{
    Animator animator;
    public float maxHealth = 10f;
    public float damage = 18f;
    public float knockbackForce = 800f;
    public float moveSpeed = 150f;
    public float maxSpeed = 0.5f;
    public bool playerDetected = false;
    private bool canMove = true;
    private bool attacking = false;

    private Vector3 playerPostion;
    private Vector2 attackDirection;

    Rigidbody2D rb;
    /*     public DamageableCharacter damageableCharacter; */

    public void resetSpeed()
    {
        maxSpeed = 0.5f;
        attacking = false;
        UnlockMovement();
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public void SetMaxHealth(float maxHealth)
    {
        {
            if (maxHealth > 0)
            {
                this.maxHealth = maxHealth;
            }
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        attackDirection = new Vector2(0, 0);
    }
    void FixedUpdate()
    {
        if (playerDetected && canMove && !attacking)
        {
            playerPostion = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector2 direction = (Vector2)(playerPostion - transform.position).normalized;
            rb.velocity = Vector2.ClampMagnitude(rb.velocity + (direction * moveSpeed * Time.deltaTime), maxSpeed);
        }
        else if (attacking)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity + (attackDirection * moveSpeed * Time.deltaTime), maxSpeed);
        }
    }

    public void OnLeap()
    {
        //wait between 0 and 2 seconds before leaping
        StartCoroutine(LeapTimer(0, 1));
    }

    IEnumerator LeapTimer(int leapLowerRange, int leapUpperRange)
    {
        //random number from 0 to 2
        float leapTime = UnityEngine.Random.Range(leapLowerRange, leapUpperRange);
        yield return new WaitForSeconds(leapTime);
        StartLeap();
    }
    public void StartLeap()
    {
        LockMovement();
        animator.SetTrigger("Leap");
    }

    public void Leap()
    {
        UnlockMovement();
        // move towards player position with leap animation and increase speed
        maxSpeed = maxSpeed * 2.5f;
        playerPostion = GameObject.FindGameObjectWithTag("Player").transform.position;
        attackDirection = (Vector2)(playerPostion - transform.position).normalized;
        attacking = true;
    }

    public void SetPlayerDetected(bool isDetected)
    {
        playerDetected = isDetected;
    }

    public void LockMovement()
    {
        canMove = false;
        attacking = false;
    }
    public void UnlockMovement()
    {
        canMove = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        IDamageable damageableObject = collider.GetComponent<IDamageable>();
        //check if damageable object not "enemy" tag

        if (damageableObject != null && collision.gameObject.tag != "Enemy")
        {
            Vector2 direction = (Vector2)(collider.transform.position - transform.position).normalized;

            Vector2 knockback = direction * knockbackForce;

            damageableObject.OnHit(damage, knockback);
        }
    }
}
