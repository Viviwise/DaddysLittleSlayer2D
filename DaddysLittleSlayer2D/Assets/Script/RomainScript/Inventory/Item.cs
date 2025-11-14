using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private InventoryItemData itemData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Inventory.Instance.AddItem(itemData, 1);
            Destroy(gameObject);
        }
    }
}
