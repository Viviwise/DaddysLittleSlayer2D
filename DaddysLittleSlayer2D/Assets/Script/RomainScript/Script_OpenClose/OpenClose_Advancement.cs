using UnityEngine;

public class OpenClose_Advancement : MonoBehaviour
{
    [SerializeField] private CanvasGroup Advancement;
    
    public void OpenCloseUi()
    {
        Advancement.alpha = Advancement.alpha > 0 ? 0 : 1;
        Advancement.blocksRaycasts = Advancement.blocksRaycasts == true ? false : true;
    }
    
}
