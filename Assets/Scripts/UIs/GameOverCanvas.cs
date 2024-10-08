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
        resultText.text = "결과창\n" + $"플레이 시간 : {(int)GameManager.PlayTime / 60}분{(GameManager.PlayTime % 60).ToString("F0")}초\n" + $"잡은 몬스터 수 : {GameManager.KillCount}마리";
    }
}
