using UnityEngine;

public class Unit : MonoBehaviour
{
  public string unitName;
  public int unitLevel;

  public int damage;

  public int maxPV;
  public int currentPV;

  public bool TakeDamage(int dmg)
  {
    currentPV -= dmg;

    if (currentPV <= 0)
      return true;
    else
      return false;
  }

  public void Heal(int amount)
  {
    currentPV += amount;
    if (currentPV > maxPV)
      currentPV = maxPV;
  }

}
  
