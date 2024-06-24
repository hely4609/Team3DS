using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIEnum
{
    
}

public class UIManager : Manager
{
    private Dictionary<UIEnum, GameObject> prefabDictionary;
    private Dictionary<UIEnum, GameObject> instanceDictionary;


    public override IEnumerator Initiate()
    {
        yield return base.Initiate();
    }
    public void GetUI(UIEnum wantUI) { }
    public void Open(UIEnum wantUI) { }
    public void Close(UIEnum wantUI) { }
    public void Toggle(UIEnum wantUI) { }
    public void ClaimError(string bar, string context, string confirm, System.Action confirmAction) { }
}
