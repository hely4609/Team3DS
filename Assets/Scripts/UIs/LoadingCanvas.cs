using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
    [SerializeField] protected Image loadingBar;
    [SerializeField] protected TextMeshProUGUI loadingProgressText;

    public void SetLoadInfo(string info, int numerator, int denominator)
    {
        if (denominator == 0) return;
        
        loadingBar.fillAmount = (float)numerator / denominator;
        loadingProgressText.text = $"{info}\n\n{(float)numerator / denominator * 100} %";
    }
}
