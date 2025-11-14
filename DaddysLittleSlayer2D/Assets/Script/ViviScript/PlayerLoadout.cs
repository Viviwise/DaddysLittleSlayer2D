using UnityEngine;

public class PlayerLoadout : MonoBehaviour
{
    public InventoryItemData[] selectedItems = new InventoryItemData[1];
    public void Sync()
    {
        Inventory.Instance.WeaponSlot.Data = selectedItems[0];
    }
}