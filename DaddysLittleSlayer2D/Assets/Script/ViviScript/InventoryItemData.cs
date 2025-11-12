using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "RPG/Item")]
public class InventoryItemData : ScriptableObject
{

    public string itemName;
    public string itemDescription;

    public ItemType itemType;

    public AttackData attack1;
    public AttackData attack2;
    
    public int healAmount;
    public Sprite itemSprite;

    public void Use()
    {
       


    }

    /*public void DeselectAllSlots()
    {
        for (int i = 0; i < inventoryItemUI.Length; i++)
        {
           
        }
    }*/
}