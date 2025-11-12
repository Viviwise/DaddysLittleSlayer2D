using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text levelText;
    public Slider pvSlider;

    public RawImage blackScreen;

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.unitLevel;
        pvSlider.maxValue = unit.maxPV;
        pvSlider.value = unit.currentPV;
    }

    public void SetPV(int pv)
    {
        pvSlider.value = pv;
    }

    public IEnumerator FadeToBlack(float duration)
    {
        
        Color color = blackScreen.color;
        color.a = 0f;
        blackScreen.color = color;
        blackScreen.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            blackScreen.color = color;
            yield return null;
        }

        color.a = 1f;
        blackScreen.color = color;
    }
}