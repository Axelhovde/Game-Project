using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public HealthBar healthBar;
    public StaminaBar staminaBar;
    public Animator animator;
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    private float health;
    private float stamina;
    Rigidbody2D rb;

    public float Health
    {
        set
        {
            health = value;
            UpdateHealthBar(health);
            if (health <= 0)
            {
                RemoveSelf();
            }
        }
        get
        {
            return health;
        }
    }
    public float Stamina
    {
        set
        {
            stamina = value;
            if (stamina <= 0)
            {
                RemoveSelf();
            }
        }
        get
        {
            return stamina;
        }
    }


    bool IDamageable.Targetable { get { return Targetable; } set { Targetable = value; } }

    public bool Targetable = true;


    public void Start()
    {

        healthBar.SetMaxHealth(maxHealth);
        staminaBar.SetMaxStamina(maxStamina);
        Health = maxHealth;
        Stamina = maxStamina;
        StartCoroutine(SetStaminaOverTime());
        rb = GetComponent<Rigidbody2D>();
    }
    public void OnHit(float damage)
    {
        Health -= damage;
        Debug.Log("Player was hit" + Health);
    }

    public void UpdateStamina(float staminaUpdate)
    {
        if (staminaUpdate + Stamina > maxStamina)
        {
            staminaBar.SetStamina(maxStamina);
            Stamina = maxStamina;
        }
        else if (staminaUpdate + Stamina < 0)
        {
            staminaBar.SetStamina(0);
            Stamina = 0;
        }
        else
        {
            staminaBar.UpdateStamina(staminaUpdate);
            Stamina += staminaUpdate;
        }
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        Debug.Log(knockback);
        Health -= damage;
        rb.AddForce(knockback);
    }
    public void RemoveSelf()
    {
        print("You died");
    }

    private void Defeated()
    {
        //animator.SetTrigger("Defeated");
    }

    private void UpdateHealthBar(float health)
    {
        healthBar.SetHealth(health);
    }

    IEnumerator SetStaminaOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (animator.GetBool("isMoving"))
            {
                UpdateStamina(1);
            }
            else
            {
                UpdateStamina(1);
            }
        }
    }
}
