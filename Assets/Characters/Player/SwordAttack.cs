using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordAttack : MonoBehaviour
{

    public float swordDamage = 3f;
    public float knockbackForce = 150f;
    public Collider2D swordCollider;
    /*     public Collider2D swordColliderUp;
        public Collider2D swordColliderDown; */
    /*     public Transform swordHitboxUp;
        public Transform swordHitboxDown; */
    public Transform player;
    Vector3 faceRight = new Vector3(0.2f, 0.1f, 0);
    Vector3 faceLeft = new Vector3(-0.2f, 0.1f, 0);
    Vector3 faceUp = new Vector3(0, 0.2f, 0);
    Vector3 faceDown = new Vector3(0, -0.1f, 0);
    Vector2 offset = new Vector2(-0.07f, 0.02f);
    Vector2 offsetUpDown = new Vector2(0f, 0f);

    public int damage = 4;

    private void Start()
    {
        swordCollider = GetComponent<Collider2D>();
    }

    public void AttackLeftRight(bool isFacingRight)
    {
        IsFacingRight(isFacingRight);
        swordCollider.enabled = true;
    }

    public void AttackUp()
    {
        gameObject.transform.localPosition = faceUp;
        swordCollider.offset = offsetUpDown;
        swordCollider.enabled = true;
    }

    public void AttackDown()
    {
        gameObject.transform.localPosition = faceDown;
        swordCollider.offset = offsetUpDown;
        swordCollider.enabled = true;
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
        /*         swordColliderUp.enabled = false;
                swordColliderDown.enabled = false; */
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        IDamageable damageableObject = collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            Vector3 parentPosition = transform.parent.position;

            Vector2 direction = (Vector2)(collider.gameObject.transform.position - parentPosition).normalized;

            Vector2 knockback = direction * knockbackForce;

            damageableObject.OnHit(damage, knockback);
        }
        else
        {
            Debug.Log("No damageable object found");
        }
    }

    void IsFacingRight(bool isFacingRight)
    {
        if (isFacingRight)
        {
            gameObject.transform.localPosition = faceRight;
            swordCollider.offset = offset;

        }
        else
        {
            gameObject.transform.localPosition = faceLeft;
            Vector2 newOffset = new Vector2(-offset.x, offset.y);
            swordCollider.offset = newOffset;
        }
    }

}
