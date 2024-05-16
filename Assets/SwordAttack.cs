using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordAttack : MonoBehaviour
{


    public Collider2D swordCollider;
    public Collider2D swordColliderUp;
    public Collider2D swordColliderDown;
    public Transform swordHitboxUp;
    public Transform swordHitboxDown;

    Vector2 rightAttackOffset;
    Vector2 upAttackOffset;
    Vector2 downAttackOffset;

    public int damage = 4;

    private void Start()
    {
        rightAttackOffset = transform.position;
        upAttackOffset = transform.InverseTransformPoint(swordHitboxUp.position);
        downAttackOffset = transform.InverseTransformPoint(swordHitboxDown.position);
    }

    public void AttackRight()
    {
        swordCollider.enabled = true;
        transform.localPosition = rightAttackOffset;
    }

    public void AttackLeft()
    {
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(-rightAttackOffset.x, rightAttackOffset.y);
    }

    public void AttackUp()
    {
        swordColliderUp.enabled = true;
        transform.localPosition = upAttackOffset;
    }

    public void AttackDown()
    {
        swordColliderDown.enabled = true;
        transform.localPosition = downAttackOffset;
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
        swordColliderUp.enabled = false;
        swordColliderDown.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                StartCoroutine(DealDamageAfterDelay(enemy, 0.1f));
            }
        }
    }
    private IEnumerator DealDamageAfterDelay(Enemy enemy, float delay)
    {
        yield return new WaitForSeconds(delay);
        enemy.Health -= damage;
    }
}
