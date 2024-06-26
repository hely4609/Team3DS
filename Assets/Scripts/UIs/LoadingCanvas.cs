using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
    [SerializeField] protected Image loadingBar;
    [SerializeField] protected TextMeshProUGUI loadingProgressText;

    public void SetLoadInfo(string info)
    {
        if (ResourceManager.resourceAmount == 0) return;
        
        loadingBar.fillAmount = ResourceManager.resourceLoadCompleted / ResourceManager.resourceAmount;
        loadingProgressText.text = $"{info}\n\n{ResourceManager.resourceLoadCompleted / ResourceManager.resourceAmount * 100} %";
    }
}
