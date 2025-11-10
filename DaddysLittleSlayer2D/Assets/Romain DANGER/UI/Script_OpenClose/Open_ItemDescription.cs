using System.Collections.Generic;
using UnityEngine;

public class Open_ItemDescription : MonoBehaviour
{
    [SerializeField] private CanvasGroup ItemDescription;
    
    public void OpenCloseUi()
    {
        ItemDescription.alpha = ItemDescription.alpha > 0 ? 1 : 1;
        ItemDescription.blocksRaycasts = ItemDescription.blocksRaycasts == false ? true : true;
    }
}
