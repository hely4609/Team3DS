using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIEnum
{
    ErrorWindow
}

public class UIManager : Manager
{
    private Dictionary<UIEnum, GameObject> prefabDictionary;
    private Dictionary<UIEnum, GameObject> instanceDictionary;

    Canvas errorCanvas;
    public override IEnumerator Initiate()
    {
        GameManager.ClaimLoadInfo("UI");
        if (prefabDictionary != null && instanceDictionary != null) yield break;

        prefabDictionary = new();
        instanceDictionary = new();

        prefabDictionary.Add(UIEnum.ErrorWindow, ResourceManager.Get(ResourceEnum.Prefab.ErrorWindow));

        GameObject errorCanvasObject = new GameObject("ErrorCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        errorCanvas = errorCanvasObject.GetComponent<Canvas>();
        errorCanvas.sortingOrder = short.MaxValue;
        errorCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        errorCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize;
        errorCanvas.transform.SetParent(GameManager.Instance.transform);

        yield return base.Initiate();
    }
    public GameObject GetUI(UIEnum wantUI) 
    {
        // 인스턴스가 있으면 inst 반환
        if(instanceDictionary.TryGetValue(wantUI, out GameObject inst) && inst != null) 
        {
            inst.SetActive(true);
            return inst;
        }
        // 없으면 프리팹은 있는지 확인
        else if(prefabDictionary.TryGetValue(wantUI, out GameObject prefab))
        {
            inst = GameObject.Instantiate(prefab, errorCanvas.transform);
            instanceDictionary.Add(wantUI, inst);
            return inst;
        }
        // 프리팹도 없으면 에러
        Debug.LogError($"Can't find UI prefab '{wantUI}'");
        return null;
    }
    public void Open(UIEnum wantUI) 
    { 
        GetUI(wantUI).SetActive(true);
    }
    public void Close(UIEnum wantUI)
    {
        GetUI(wantUI).SetActive(false);
    }
    public void Toggle(UIEnum wantUI)
    {
        GameObject gotUI = GetUI(wantUI);
        gotUI.SetActive(!gotUI.activeInHierarchy);
    }
    public void ClaimError(string bar, string context, string confirm, System.Action confirmAction) 
    {
        GameObject errorWindow = GetUI(UIEnum.ErrorWindow);
        errorWindow.GetComponent<ErrorWindow>().SetText(bar, context, confirm, confirmAction + (() => { GameObject.Destroy(errorWindow); }));
    }
}
