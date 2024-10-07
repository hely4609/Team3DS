using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleManager : Manager
{
    bool isChanging;
    int languageInt;
    public void ChangeLocale(int index)
    {
        if (isChanging)
            return;
        GameManager.Instance.StartCoroutine(ChangeRoutine(index));
    }

    IEnumerator ChangeRoutine(int index)
    {
        isChanging = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        isChanging = false;
    }

    public override IEnumerator Initiate()
    {

        languageInt = 0;
        ChangeLocale(0);
        return ChangeRoutine(languageInt);
    }
    public void Initialize()
    {
    }
}
