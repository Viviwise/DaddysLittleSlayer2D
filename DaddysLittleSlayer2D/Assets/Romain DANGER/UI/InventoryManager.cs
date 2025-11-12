using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot;

    public ItemSO[] itemSOs;
    
    
    void Update()
    {
        if (Input.GetButtonDown("Inventory") &&  menuActivated)
        {
            InventoryMenu.SetActive(false);
            menuActivated = false;
        }
        
        else if (Input.GetButtonDown("Inventory") &&  !menuActivated)
        {
            InventoryMenu.SetActive(true);
            menuActivated = true;
        }
    }

    public bool UseItem(string itemName)
    {
        for (int i = 0; i < itemSOs.Length; i++)
        {
            if (itemSOs[i].itemName == itemName)
            {
                bool usable = itemSOs[i].UseItem();
                return usable;
            }
            return false;
        }
        return false;
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false &&  itemSlot[i].name == name || itemSlot[i].quantity == 0)
            {
                int LeftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                if (LeftOverItems > 0)
                    return LeftOverItems;
            }
            
        }
        return quantity;
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectShader.SetActive(false);
            itemSlot[i].thisItemSelected =  false;
        }
    }
}
