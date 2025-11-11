using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
   public Text nameText;
   public Text levelText;
   public Slider pvSlider;

   public void SetHUD(Unit unit)
   {
      nameText.text = unit.unitName;
      levelText.text = "Lvl "  + unit.unitLevel;
      pvSlider.maxValue = unit.maxPV;
      pvSlider.value = unit.currentPV;
   }


   public void SetPV(int pv)
   {
       pvSlider.value = pv;
   }
}
