using UnityEngine;

public class OpenClose_Unequip : MonoBehaviour
{
   [SerializeField] private CanvasGroup Unequip;
   
       public void OpenCloseUi()
       {
           Unequip.alpha = Unequip.alpha > 0 ? 0 : 1;
           Unequip.blocksRaycasts = Unequip.blocksRaycasts == true ? false : true;
           
       }
}
