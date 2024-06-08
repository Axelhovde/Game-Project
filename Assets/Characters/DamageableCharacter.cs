using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DamageableCharacter : MonoBehaviour, IDamageable
{
    public GameObject healthText;
    Animator animator;
    public Rigidbody2D rb;
    public Collider2D physicsCollider;
    public LevelBar levelBar;
    public float maxHealth;
    public int xpGiven;

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
    public float GetHealth()
    {
        return health;
    }
    public void SetHealth(float health)
    {
        {
            if (this.health < health)
            {
                animator.SetTrigger("Damaged");
                RectTransform textTransform = Instantiate(healthText).GetComponent<RectTransform>();
                textTransform.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            }
            this.health = health;
            if (this.health <= 0)
            {
                targetable = false;
                animator.SetTrigger("Defeated");
                Debug.Log("Enemy defeated, and " + xpGiven + " xp given.");
                levelBar.AddXp(xpGiven);
            }
        }
    }

    public bool Targetable
    {
        get
        { return targetable; }
        set
        {
            if (enableSimulation)
            {
                rb.simulated = value;
            }
            targetable = value;
            physicsCollider.enabled = value;
        }
    }
    public void SetTargetableFalse()
    {
        Targetable = false;
    }
    public float health;
    public bool targetable = true;
    public bool enableSimulation;
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCollider = GetComponent<Collider2D>();
        health = maxHealth;
        levelBar = GameObject.Find("LvlBar").GetComponent<LevelBar>();

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
        SetHealth(GetHealth() - damage);
        rb.AddForce(knockback);
        animator.SetTrigger("Damaged");
    }

    public void OnHit(float damage)
    {
        SetHealth(GetHealth() - damage);
    }

}
