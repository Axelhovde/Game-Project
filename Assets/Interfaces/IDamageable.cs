using UnityEngine;

public interface IDamageable
{
    public float GetHealth();
    public void SetHealth(float health);
    public bool Targetable { get; set; }
    public void OnHit(float damage, Vector2 knockback);
    public void OnHit(float damage);
    public void RemoveSelf();
}