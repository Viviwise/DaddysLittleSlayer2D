using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Identity")]
    public string unitName;
    public int unitLevel;

    [Header("Combat stats")]
    public int damage;       
    public int maxPV;
    public int currentPV;
    
    public Animator animator;

    [Header("Equipment")]
    public ItemData equippedItem; 

    private void Awake()
    {
      
        if (currentPV <= 0)
            currentPV = maxPV;
    }
    
    public bool TakeDamage(int dmg)
    {
        currentPV -= dmg;
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (currentPV <= 0)
        {
            currentPV = 0;
            return true;
        }

        return false;
    }

    public void Heal(int amount)
    {
        currentPV += amount;
        if (currentPV > maxPV)
            currentPV = maxPV;
    }
}