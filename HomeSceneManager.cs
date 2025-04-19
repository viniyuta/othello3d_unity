using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeSceneManager : MonoBehaviour
{
    [SerializeField] LanguageManager localizationManager;

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnOfflineButton()
    {
        LoadScene("OfflineGameScene");
    }

    public void OnOnlineButton()
    {
        LoadScene("OnlineGameScene");
    }

    public void OnLanguageButton()
    {
        int nextId = localizationManager.NextId();
        localizationManager.ChangeLocale(nextId);
    }
}
