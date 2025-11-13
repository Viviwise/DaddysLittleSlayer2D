using Unity.VisualScripting;
using UnityEngine;

public class OpenClose_Inventory : MonoBehaviour
{
    
    [SerializeField] private CanvasGroup Inventory;
    
    public void OpenCloseUi()
    {
        Inventory.alpha = Inventory.alpha > 0 ? 0 : 1;
        Inventory.blocksRaycasts = Inventory.blocksRaycasts == true ? false : true;
    }
    
}