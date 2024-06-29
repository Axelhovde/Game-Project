using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    public HealthBar healthBar;
    public StaminaBar staminaBar;
    public LevelBar levelBar;
    public Animator animator;
    private int level = 0;
    private int xp = 0;
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    private float health;
    private float stamina;
    Rigidbody2D rb;
    public PlayerController playerController;
    private bool invinicible = false;

    public void SetInvincibility(bool invinicible)
    {
        this.invinicible = invinicible;
    }
    public int GetLevel()
    {
        return level;
    }
    public void SetLevel(int level)
    {
        this.level = level;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public int GetXp()
    {
        return xp;
    }
    public void SetXp(int xp)
    {
        this.xp = xp;
    }

    public float GetHealth()
    {
        return health;
    }
    public void SetHealth(float health)
    {
        this.health = health;
        UpdateHealthBar(health);
        if (health <= 0)
        {
            RemoveSelf();
        }
    }
    public void SetStamina(float stamina)
    {
        this.stamina = stamina;
        staminaBar.SetStamina(stamina);
    }
    public void SetPosition(float x, float y)
    {
        transform.position = new Vector2(x, y);
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
        levelBar.SetLevel(level);
        levelBar.SetMaxXp();
        levelBar.SetXp(xp);
        SetStamina(maxStamina);
        SetHealth(maxHealth);
        StartCoroutine(SetStaminaOverTime());
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnHit(float damage)
    {
        if (!invinicible)
        {
            SetHealth(GetHealth() - damage);
            playerController.StartBlinkEffect();
        }
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
            staminaBar.AddStamina(staminaUpdate);
            Stamina += staminaUpdate;
        }
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        if (!invinicible)
        {
            SetHealth(GetHealth() - damage);
            rb.AddForce(knockback);
            playerController.StartBlinkEffect();
        }
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
