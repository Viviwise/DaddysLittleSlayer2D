using UnityEngine;

public class PlayerLoadout : MonoBehaviour
{
    public InventoryItemData[] selectedItems = new InventoryItemData[2];
    public void Sync()
    {
        Inventory.Instance.WeaponSlot.Data = selectedItems[0];
    }
}