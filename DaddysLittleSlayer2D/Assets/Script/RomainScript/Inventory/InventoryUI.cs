using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : MonoBehaviour 
{ 
    [SerializeField] private Inventory inventory; 

    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private InventoryItemData Data;

    public int quantity;
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text quantityText;

    public void Sync() 
    { 
        foreach (Transform child in transform) 
            Destroy(child.gameObject);
        
        foreach ((InventoryItemData itemPair, int quantity) in inventory.items) 
        { 
            GameObject uiItem = Instantiate(uiPrefab, transform); 
            inventoryItemUI uiItemComponent = uiItem.GetComponent<inventoryItemUI>();
            uiItemComponent.SetReferences(itemDescriptionImage, itemDescriptionText, itemDescriptionNameText);
            uiItemComponent.SetItem(itemPair, quantity); 
        }

        EmptySlot();
    }
    public void EmptySlot()
    {
        itemDescriptionImage.sprite = null ;
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
        itemDescriptionImage.sprite = null;
        Debug.Log("Empty Slot");
    }

    public void DeselectAllSlots() 
    { 
        foreach (Transform child in transform) 
        { 
            inventoryItemUI uiItemComponent = child.GetComponent<inventoryItemUI>(); 
            uiItemComponent.selectShader.SetActive(false); 
            uiItemComponent.thisItemSelected = false;
        }
        EmptySlot();
    }

}
