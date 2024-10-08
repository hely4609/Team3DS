using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;

    public void GoTitle()
    {
        GameManager.Instance.GoTitle();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetResultText()
    {
        if(GameManager.Instance.BuildingManager.generator == null) GameManager.Instance.BuildingManager.generator = GameObject.FindObjectOfType<EnergyBarrierGenerator>();
        resultText.text = "���â\n" + $"�÷��� �ð� : {(int)GameManager.Instance.BuildingManager.generator.PlayTime / 60}��{(GameManager.Instance.BuildingManager.generator.PlayTime % 60).ToString("F0")}��\n" + $"���� ���� �� : {GameManager.Instance.BuildingManager.generator.KillCount}����";
    }
}
