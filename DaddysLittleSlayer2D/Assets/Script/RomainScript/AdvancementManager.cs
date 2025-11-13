using UnityEngine;

public class AdvancementManager : MonoBehaviour
{
    public GameObject AdvancementMenu;
    private bool advancementActivated;
    
    void Update()
    {
        if (Input.GetButtonDown("Advancement") &&  advancementActivated)
        {
            AdvancementMenu.SetActive(false);
            advancementActivated = false;
           
        }
        
        else if (Input.GetButtonDown("Advancement") &&  !advancementActivated)
        {
            AdvancementMenu.SetActive(true);
            advancementActivated = true;
        }
    }
}
        
