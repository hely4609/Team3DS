using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class GameOverCanvas : MonoBehaviour
{
    public void GoTitle()
    {
        GameManager.Instance.GoTitle();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetResultText()
    {
        // Get our GlobalVariablesSource
        var source = LocalizationSettings
            .StringDatabase
            .SmartFormatter
            .GetSourceExtension<PersistentVariablesSource>();
        // Get the specific global variable
        var Miniute =
            source["ResultGroup"]["Min"] as IntVariable;
        var Second =
    source["ResultGroup"]["Sec"] as IntVariable;
        var Monster =
    source["ResultGroup"]["Monster"] as IntVariable;

        Monster.Value = GameManager.Instance.BuildingManager.generator.KillCount;
        Miniute.Value = (int)GameManager.Instance.BuildingManager.generator.PlayTime / 60;
        Second.Value = (int)GameManager.Instance.BuildingManager.generator.PlayTime % 60;
    }
}
