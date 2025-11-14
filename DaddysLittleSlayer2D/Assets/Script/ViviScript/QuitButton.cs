using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
