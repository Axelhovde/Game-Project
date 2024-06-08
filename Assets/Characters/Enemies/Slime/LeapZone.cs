using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapZone : MonoBehaviour
{
    Collider2D playerCollider;

    Slime slime;
    bool canLeap = true;
    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        slime = GetComponentInParent<Slime>();
    }
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider == playerCollider)
        {
            if (canLeap)
            {
                slime.OnLeap();
                canLeap = false;
                StartCoroutine(LeapTimer(3, 5));
            }
        }
    }
    //timer function making the slime leap every 3 seconds
    IEnumerator LeapTimer(int leapLowerRange, int leapUpperRange)
    {
        //random number from 3 to 6
        float leapTime = Random.Range(leapLowerRange, leapUpperRange);
        yield return new WaitForSeconds(leapTime);
        canLeap = true;
    }
}
