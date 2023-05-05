using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;
using UnityEngine.SceneManagement;

public class AppsFlyerManager : MonoBehaviour
{
    [SerializeField] AppsFlyerObjectScript AppsFlyerObj;
    void Start()
    {
        AppsFlyerObj.onConversionDataSuccess("First");
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
