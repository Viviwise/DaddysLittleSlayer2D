using System;
using UnityEngine;
using UnityEngine.SceneManagement;



public class DeadUIMenu : MonoBehaviour
{
    public void ReloadGame()
    {
        SceneManager.LoadScene("ViviScene");
    }
}
