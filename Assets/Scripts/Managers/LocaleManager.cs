using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

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
        //yield return null;
        isChanging = false;
    }
    public void LoadLocale(string languageIdentifier)
    {
        LocaleIdentifier localeCode = new LocaleIdentifier(languageIdentifier);
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            Locale aLocale = LocalizationSettings.AvailableLocales.Locales[i];
            LocaleIdentifier anIdentifier = aLocale.Identifier;
            if (anIdentifier == localeCode)
            {
                LocalizationSettings.SelectedLocale = aLocale;
            }
        }
    }
    public override IEnumerator Initiate()
    {
        languageInt = 0;
        ChangeLocale(languageInt);
        yield return null;
    }
    public void Initialize()
    {
    }

    public string LocaleNameSet(string str)
    {
        LocalizedString localizedString = new LocalizedString() { TableReference = "ChangeableTable", TableEntryReference = str };
        var stringOperation = localizedString.GetLocalizedStringAsync();
        Debug.Log($"{stringOperation.IsDone} / {stringOperation.Status}");
        if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded)
        {
            return stringOperation.Result;
        }
        return "���峲";
    }
}
