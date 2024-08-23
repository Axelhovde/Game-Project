using System;
using Unity.VisualScripting;
using UnityEngine;

public class BowRotation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponentInParent<PlayerController>();
        if(playerController == null)
        {
            Debug.LogError("BowRotation script must be a child of the PlayerController object");
        }
    }

    void Update()
    {
        // Call the GetSelectedWeapon method and check if the returned value equals "Bow"
        if (playerController.GetSelectedWeapon().Equals("Bow"))
        {
            RotateBowTowardsMouse();
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }

    void RotateBowTowardsMouse()
    {
        // Get the mouse position in world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction from the bow to the mouse
        Vector2 direction = mousePosition - transform.position;

        // Calculate the angle between the bow's position and the mouse position
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (direction.x < 0)
        {
            angle += 180;
        }
        // Apply the rotation to the bow's transform
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
