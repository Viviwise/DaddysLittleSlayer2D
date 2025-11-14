using UnityEngine;

public class PlayerLoadout : MonoBehaviour
{
    public InventoryItemData[] selectedItems = new InventoryItemData[2];
    
    public void Sync(InventoryItemData Data)
    {
        InventoryItemData itemToEquip = Data;
    }
}