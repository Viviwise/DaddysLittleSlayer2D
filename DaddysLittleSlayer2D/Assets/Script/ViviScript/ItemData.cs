using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "RPG/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;

    public AttackData attack1;
    public AttackData attack2;
    
    public int healAmount;
}