using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LvlDisplay : MonoBehaviour
{
    //update image to display the current level
    public void UpdateLvlDisplay(int lvl)
    {
        Image number = GetComponent<Image>();
        // Load all the sprites from the spritesheet
        Sprite[] sprites = Resources.LoadAll<Sprite>("UI/Numbers");
        // Find the sprite for the new level
        Sprite newSprite = Array.Find(sprites, sprite => sprite.name == $"Numbers_{lvl}");
        // Set the sprite of the Image component
        number.sprite = newSprite;
    }
}
