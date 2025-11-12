using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
public class inventoryItemUI : MonoBehaviour, IPointerClickHandler
{ 
    
    //=============== ITEM DATA ===============\\

    public string itemName; 
    public int quantity; 
    public Sprite itemSprite; 
    public bool isFull; 
    public string itemDescription; 
    public Sprite emptySprite; 
    [SerializeField] private int maxNumberOfItems; 
    
    //=============== ITEM SLOT ===============\\
    [SerializeField] private TMP_Text quantityText; 
    [SerializeField] private Image itemImage;

    //=============== ITEM DESCRIPTION SLOT ===============\\
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    public GameObject selectShader; 
    public bool thisItemSelected; 
    private InventoryItemData data; 
    public static int Length { get; private set; } 
    public void SetItem(InventoryItemData itemPair, int quantity) 
    { 
        data = itemPair;
        itemName = itemPair.itemName; 
        itemSprite = itemPair.itemSprite; 
        itemDescription = itemPair.itemDescription; 
        this.quantity = quantity; 
        itemImage.sprite = itemSprite; 
        itemImage.gameObject.SetActive(true);
        quantityText.text = quantity.ToString(); 
        quantityText.enabled = true; 
    }

    public void SetReferences(Image itemImage, TMP_Text description, TMP_Text name)
    {
        itemDescriptionImage = itemImage;
        itemDescriptionText = description;
        itemDescriptionNameText = name;

    }

  

    public void OnPointerClick(PointerEventData eventData)
    {
       

        if (thisItemSelected)
        {
            data.Use();
            this.quantity -= 1;
            quantityText.text = this.quantity.ToString();
            if (this.quantity <= 0)
            {
                
            }
        }
        else
        {
            Inventory.Instance.inventoryUI.DeselectAllSlots();
            selectShader.SetActive(true);
            thisItemSelected = true;
            itemDescriptionNameText.text = itemName;
            itemDescriptionText.text = itemDescription;
            itemDescriptionImage.sprite = itemSprite;
            if (itemDescriptionImage.sprite == null)
            {
                itemDescriptionImage.sprite = emptySprite;
            }
        }

    }
    
    /*public void OnLeftClick()
    {
        if (thisItemSelected)
        {
            bool usable = InventoryManager.UseItem(itemName);
            this.quantity -= 1;
            quantityText.text = this.quantity.ToString();
            if (this.quantity <= 0)
            {
                EmptySlot();
            }
        }
        else
        {
            InventoryManager.DeselectAllSlots();
            selectShader.SetActive(true);
            thisItemSelected = true;
            itemDescriptionNameText.text = itemName;
            itemDescriptionText.text =  itemDescription;
            itemDescriptionImage.sprite = itemSprite;
            if (itemDescriptionImage.sprite == null)
            {
                itemDescriptionImage.sprite = emptySprite;
            }
        }
        
    }

    private void EmptySlot()
    {
        quantityText.enabled = false;
        itemImage.sprite = emptySprite;
        
        itemDescriptionNameText.text = "";
        itemDescriptionText.text =  "";
        itemDescriptionImage.sprite = emptySprite;
    }

    public void OnRightClick()
    {
        selectShader.SetActive(false);
    }*/
}
