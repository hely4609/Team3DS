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
        resultText.text = "결과창\n" + $"플레이 시간 : {(int)GameManager.Instance.BuildingManager.generator.PlayTime / 60}분{(GameManager.Instance.BuildingManager.generator.PlayTime % 60).ToString("F0")}초\n" + $"잡은 몬스터 수 : {GameManager.Instance.BuildingManager.generator.KillCount}마리";
    }
}
