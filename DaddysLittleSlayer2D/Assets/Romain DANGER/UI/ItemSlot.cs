using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{

    //=============== ITEM DATA ===============\\
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;
    
    [SerializeField]
    private int maxNumberOfItems;

    //=============== ITEM SLOT ===============\\
    [SerializeField] private TMP_Text quantityText;

    [SerializeField] private Image itemImage;
    
    
    //=============== ITEM DESCRIPTION SLOT ===============\\
    
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;

    public GameObject selectShader;
    public bool thisItemSelected;
    
    private InventoryManager InventoryManager;

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        //Check to see if the slot is full//
        if (isFull)
            return quantity;
        
        //UPDATE NAME//
        this.itemName = itemName;
        
        //UPDATE IMAGE//
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;
        
        //UPDATE DESCRIPTION//
        this.itemDescription = itemDescription;

        //UPDATE QUANTITY//
        this.quantity += quantity;
        if (this.quantity >= maxNumberOfItems)
        {
            quantityText.text = quantity.ToString();
            quantityText.enabled = true;
            isFull = true;
            
            //RETURN THE LEFT OVER//
            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            return extraItems;
        }
        
        //UPDATE QUANTITY TEXT//
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;

        return 0;
    }

    private void Start()
    {
        InventoryManager = GameObject.Find("Inventory Canvas").GetComponent<InventoryManager>();
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }
    
    public void OnLeftClick()
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
    }
}
