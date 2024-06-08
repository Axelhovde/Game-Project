using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    Collider2D playerCollider;
    Collider2D enemyCollider;
    Slime slime;
    // Start is called before the first frame update
    void Start()
    {
        enemyCollider = GetComponent<Collider2D>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        slime = GetComponentInParent<Slime>();
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider == playerCollider)
        {
            slime.SetPlayerDetected(true);
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider == playerCollider)
        {
            slime.SetPlayerDetected(false);
        }
    }

}
