using ResourceEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UIEnum
{
    ErrorWindow, SignInCanvas, SetNicknameCanvas, BeInvitedWindow, CharacterUICanvas, RoomButton, Minimap
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
        prefabDictionary.Add(UIEnum.SignInCanvas, ResourceManager.Get(ResourceEnum.Prefab.SignInCanvas));
        prefabDictionary.Add(UIEnum.SetNicknameCanvas, ResourceManager.Get(ResourceEnum.Prefab.SetNicknameCanvas));
        prefabDictionary.Add(UIEnum.BeInvitedWindow, ResourceManager.Get(ResourceEnum.Prefab.BeInvitedWindow));
        prefabDictionary.Add(UIEnum.CharacterUICanvas, ResourceManager.Get(ResourceEnum.Prefab.CharacterUICanvas));
        prefabDictionary.Add(UIEnum.RoomButton, ResourceManager.Get(ResourceEnum.Prefab.RoomButton));
        prefabDictionary.Add(UIEnum.Minimap, ResourceManager.Get(ResourceEnum.Prefab.Minimap));

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
        if(instanceDictionary.TryGetValue(wantUI, out GameObject inst)) 
        {
            if(inst == null)
            {
                if (prefabDictionary.TryGetValue(wantUI, out GameObject prefab))
                {
                    inst = GameObject.Instantiate(prefab);
                    instanceDictionary[wantUI] = inst;
                    return inst;
                }
            }
            else
            {
                return inst;
            }
        }
        // 없으면 프리팹은 있는지 확인
        else if(prefabDictionary.TryGetValue(wantUI, out GameObject prefab))
        {
            inst = GameObject.Instantiate(prefab);
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
    public void ClaimError(string bar, string context, string confirm, System.Action confirmAction = null) 
    {
        if (prefabDictionary.TryGetValue(UIEnum.ErrorWindow, out GameObject prefab))
        {
            GameObject errorWindow = GameObject.Instantiate(prefab, errorCanvas.transform);
            errorWindow.GetComponent<ErrorWindow>().SetText(bar, context, confirm, confirmAction + (() => 
            { 
                GameObject.Destroy(errorWindow); 
                if(GameManager.IsGameStart) Cursor.lockState = CursorLockMode.Locked; 
            }));
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ClaimInviteWindow()
    {
        
    }
}
