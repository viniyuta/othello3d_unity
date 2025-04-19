using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport.Error;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SocialPlatforms;

public class LanguageManager : MonoBehaviour
{
    private int localesCount;
    private int currentLocaleId = 1;

    // Start is called before the first frame update
    void Awake()
    {
        localesCount = LocalizationSettings.AvailableLocales.Locales.Count;
        DontDestroyOnLoad(gameObject);
        ChangeLocale(currentLocaleId);
    }

    private bool active = false;

    public void ChangeLocale(int localeId)
    {
        if (active) return;
        StartCoroutine(SetLocale(localeId));
    }

    private IEnumerator SetLocale(int localeId)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
        currentLocaleId = localeId;
        active = false;
    }

    public int NextId()
    {
        return (currentLocaleId + 1) % localesCount;
    }
}
