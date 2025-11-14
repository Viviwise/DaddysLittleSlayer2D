using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoadout : MonoBehaviour
{
    public InventoryItemData selectedWeapon;
    public List<InventoryItemData> selectedConsumables;

    private void Awake()
    {
        selectedConsumables = new List<InventoryItemData>();
    }

    private void OnEnable()
    {
        Inventory.Instance.WeaponSlot.OnItemSelected += OnWeaponChanged;
        Inventory.Instance.ConsumableSlot.OnItemSelected += OnConsumableAdded;

    }

    private void OnDisable()
    {
        Inventory.Instance.WeaponSlot.OnItemSelected -= OnWeaponChanged;
        Inventory.Instance.ConsumableSlot.OnItemSelected -= OnConsumableAdded;

    }

    public void OnWeaponChanged(InventoryItemData selectedItem)
    {
        selectedWeapon = selectedItem;
    }

    public void OnConsumableAdded(InventoryItemData selectedItem)
    {
        selectedConsumables.Add(selectedItem);
    }
}