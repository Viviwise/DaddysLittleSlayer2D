using UnityEngine;

public class HideInDeath : MonoBehaviour
{
    public Unit playerUnit;
    void Update()
    {
        if (playerUnit.currentPV <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
    }
}
