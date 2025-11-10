using UnityEngine;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public StatToChange statToChange =  new StatToChange();
    public int amountToChangeStat;
    public AttributeToChange attributeToChange = new AttributeToChange();
    public int amountToChangeAttribute;

    public bool UseItem()
    {
        Unit unit = GameObject.Find("Unit").GetComponent<Unit>();
        if (statToChange == StatToChange.health)
        {
            if (unit.currentPV == unit.maxPV)
            {
                return false;
            }
            else
            {
                unit.Heal(amountToChangeStat);
                return true;
            }


        }

        if (statToChange == StatToChange.strength)
        {
            if (unit.currentPV == unit.maxPV)
            {
                return false;
            }
            else
            {
                unit.TakeDamage(amountToChangeStat);
                return true;
            }
        }

        return false;
    }

    public enum StatToChange
    {
        none,
        health,
        strength,
    };
    
    public enum AttributeToChange
    {
        none,
        health,
        strength,
    };
}
