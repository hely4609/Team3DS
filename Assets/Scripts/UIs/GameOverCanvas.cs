using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;

    public void GoTitle()
    {
        GameManager.Instance.GoTitle();
    }

    public void SetResultText()
    {
        resultText.text = "���â\n" + $"�÷��� �ð� : {(int)GameManager.Instance.BuildingManager.generator.PlayTime / 60}��{(GameManager.Instance.BuildingManager.generator.PlayTime % 60).ToString("F0")}��\n" + $"���� ���� �� : {GameManager.Instance.BuildingManager.generator.KillCount}����";
    }
}
