using UnityEngine;

public class Arrow : MonoBehaviour
{
    /*     public float baseSpeed = 2.5f; */
    private float lifetime;
    private Vector2 direction;
    private float speed;
    private int damage;
    private float knockbackForce;

    // Set the direction and speed of the arrow
    public void SetDirectionAndSpeed(Vector2 direction, float speed, int damage, float knockbackForce, float lifetime)
    {
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.lifetime = lifetime;

        // Rotate the arrow to face the direction it is moving
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void Update()
    {
        // Move the arrow in the set direction
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void Start()
    {
        // Destroy the arrow after it has existed for its lifetime
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        IDamageable damageableObject = collider.GetComponent<IDamageable>();

        if (damageableObject != null && collision.gameObject.tag == "Enemy")
        {
            damageableObject.OnHit(damage, direction * knockbackForce);
        }

        if (collision.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }
}
