using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    string Name { get; }
    string WeaponType { get; }
    Sprite Image { get; }
    void OnPickup();
    void OnDrop();

}

public class InventoryEventArgs : EventArgs
{
    public IInventoryItem Item;
    public InventoryEventArgs(IInventoryItem item)
    {
        Item = item;
    }
}