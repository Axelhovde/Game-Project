using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordAttack : MonoBehaviour
{

    public float knockbackForce = 150f;
    public PolygonCollider2D swordCollider;
    Vector3 initialScale;


    public int damage = 4;

    private void Start()
    {
        /* swordCollider = GetComponent<PolygonCollider2D>(); */
        initialScale = swordCollider.transform.localScale;
    }
    public void Attack()
    {
        print("Attackupordown");
        swordCollider.enabled = true;
    }
    public void Attack(bool isFacingRight)
    {
        // Get the current scale of the collider
        Vector3 currentScale = swordCollider.transform.localScale;
        if (!isFacingRight && currentScale == initialScale)
        {
            swordCollider.transform.localScale = new Vector3(currentScale.x * -1f, currentScale.y, currentScale.z);
        }
        else if (isFacingRight && currentScale != initialScale)
        {
            swordCollider.transform.localScale = initialScale;
        }
        swordCollider.enabled = true;
    }



    public void StopAttack()
    {
        swordCollider.enabled = false;
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
        if (collider.tag == "Destructible")
        {
            print("Tree Hit");
            TreeHandler treeHandler = collider.GetComponentInParent<TreeHandler>();
            if (treeHandler != null)
            {
                treeHandler.ShakeTree();
            }
        }
    }


}
