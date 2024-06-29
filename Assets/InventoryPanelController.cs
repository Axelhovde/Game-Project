using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryPanelController : MonoBehaviour
{
    public Inventory Inventory;
    private int scrollValue = 1;
    private float scroll = 0;
    PlayerInputActions inputActions;
    private Image image;
    public Sprite _SelectedImage;
    public Sprite _NotSelectedImage;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Scroll.performed += x => scroll = x.ReadValue<float>();
    }

    private void Update()
    {
        if (scroll > 0)
        {
            if (scrollValue < 9)
            {
                scrollValue++;
            }
            else
            {
                scrollValue = 1;
            }
            ChangeSelectedSlot(scrollValue);
        }
        else if (scroll < 0)
        {
            if (scrollValue > 1)
            {
                scrollValue--;
            }
            else
            {
                scrollValue = 9;
            }
            ChangeSelectedSlot(scrollValue);
        }
    }

    #region - Enable/Disable Input Actions -
    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    #endregion

    void Start()
    {
        Inventory.ItemAdded += InventoryScript_ItemAdded;
        ChangeSelectedSlot(scrollValue);
    }

    private void ChangeSelectedSlot(int scrollValue)
    {
        //get child called slot (n) and change image
        if (image != null)
        {
            image.sprite = _NotSelectedImage;
        }
        image = transform.Find("Slot (" + scrollValue + ")").GetComponent<Image>();
        image.sprite = _SelectedImage;

    }



    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        foreach (Transform slot in transform)
        {
            Transform item = slot.GetChild(0);
            Image image = item.GetComponent<Image>();
            if (!image.enabled)
            {
                image.enabled = true;
                image.sprite = e.Item.Image;

                //TODO: Store reference to item
                break;
            }
        }
    }
}
