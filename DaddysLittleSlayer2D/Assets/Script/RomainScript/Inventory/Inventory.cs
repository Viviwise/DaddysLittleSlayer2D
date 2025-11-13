using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public static Inventory Instance;
    public InventoryUI inventoryUI;

    [SerializeField] public inventoryItemUI ConsumableSlot;
    [SerializeField] public inventoryItemUI WeaponSlot;

    public Dictionary<InventoryItemData, int> items = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void RemoveItem(InventoryItemData inventoryItem, int quantity)
    {
        if (items.ContainsKey(inventoryItem))

        {
            items[inventoryItem] -= quantity;
            if (items[inventoryItem] <= 0) items.Remove(inventoryItem);
        }
    }

    public void AddItem(InventoryItemData inventoryItem, int quantity)
    {
        if (items.ContainsKey(inventoryItem))
        {
            items[inventoryItem] += quantity;
        }
        else
        {
            items.Add(inventoryItem, quantity);
        }
    }
    public bool HasItem(InventoryItemData inventoryItem) => items.ContainsKey(inventoryItem);
    public int GetQuantity(InventoryItemData inventoryItem)
    {
        if (items.ContainsKey(inventoryItem))
        {
            return items[inventoryItem];
        }
        return 0;
    }

}

