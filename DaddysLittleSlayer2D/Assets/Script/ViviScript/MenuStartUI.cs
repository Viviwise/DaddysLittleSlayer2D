using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuStartUI : MonoBehaviour
{
    public RawImage blackScreen;

    public void StartButton()
    {
        StartCoroutine(StartGameSequence());
    }

    private IEnumerator StartGameSequence()
    {
        yield return StartCoroutine(FadeToBlack(2f));

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("ViviScene");
    }

    private IEnumerator FadeToBlack(float duration)
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

