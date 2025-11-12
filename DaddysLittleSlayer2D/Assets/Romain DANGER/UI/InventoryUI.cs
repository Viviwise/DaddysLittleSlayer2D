using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : MonoBehaviour 
{ 
    [SerializeField] private Inventory inventory; 

    [SerializeField] private GameObject uiPrefab;

    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    
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
    private void EmptySlot()
    {
        //quantityText.enabled = false;
        itemDescriptionImage.sprite = null ;

        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
        itemDescriptionImage.sprite = null;
    }

    public void DeselectAllSlots() 
    { 
        foreach (Transform child in transform) 
        { 
            inventoryItemUI uiItemComponent = child.GetComponent<inventoryItemUI>(); 
            uiItemComponent.selectShader.SetActive(false); 
            uiItemComponent.thisItemSelected = false; 
        }
    }
}
