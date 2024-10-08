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
        resultText.text = "���â\n" + $"�÷��� �ð� : {(int)GameManager.PlayTime / 60}��{(GameManager.PlayTime % 60).ToString("F0")}��\n" + $"���� ���� �� : {GameManager.KillCount}����";
    }
}
