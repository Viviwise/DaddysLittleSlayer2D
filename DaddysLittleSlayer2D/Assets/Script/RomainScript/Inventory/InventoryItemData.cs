using System;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "LupeniItem")]
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
        
        switch
            (itemType)
        {
            case ItemType.Weapon:
                UseWeapon();
                break;
            case ItemType.Consumable:
                UseConsumable();
                break;
          
        }


    }

    public void UseWeapon()
    {
        Inventory.Instance.WeaponSlot.SetItem(this, Inventory.Instance.GetQuantity(this));
    }

    public void UseConsumable()
    {
        Inventory.Instance.ConsumableSlot.SetItem(this, Inventory.Instance.GetQuantity(this));

    }
}