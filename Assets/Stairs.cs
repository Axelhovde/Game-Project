using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stairs : MonoBehaviour
{

    public PlayerController playerController;
    //if player enters stairs, player move in y direction as much as the movement in x direction
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player entered stairs");
            playerController.SetInStairs(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player left stairs");
            playerController.SetInStairs(false);
        }
    }
}
