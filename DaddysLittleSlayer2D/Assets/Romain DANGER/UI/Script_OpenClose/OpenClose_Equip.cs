using System;
using UnityEngine;

public class OpenClose_Equip : MonoBehaviour
{
    [SerializeField] private CanvasGroup Equip;
    
    public void OpenCloseUi()
    {
        Equip.alpha = Equip.alpha > 0 ? 0 : 1;
        Equip.blocksRaycasts = Equip.blocksRaycasts == true ? false : true;
        
    }
    
    
    
    
}
