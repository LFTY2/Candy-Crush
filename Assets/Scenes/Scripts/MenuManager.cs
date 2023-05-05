using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LoadGameSceane()
    {
        SceneManager.LoadScene("Game");
    }
    public void LoadDataSceane()
    {
        SceneManager.LoadScene("Data");
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
