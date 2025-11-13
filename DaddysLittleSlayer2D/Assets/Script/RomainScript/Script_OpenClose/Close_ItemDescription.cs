using UnityEngine;

public class Close_ItemDescription : MonoBehaviour
{
    [SerializeField] private CanvasGroup ItemDescription;
    
    public void OpenCloseUi()
    {
        ItemDescription.alpha = ItemDescription.alpha > 1 ? 1 : 0;
        ItemDescription.blocksRaycasts = ItemDescription.blocksRaycasts == true ? false : true;
    }
    
    
}
