using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : Manager
{
    //2CB 참고.
    //Layer설정으로관리
    //MiniMapCamera의 CullingMask로 보고싶은 Layer조정가능
    //RenderTexture필요
    public Camera miniMapCamera;
    Vector3 pos;
    Vector3 rot;
    public override IEnumerator Initiate() { yield return null; }

    public override void ManagerUpdate(float deltaTime)
    {
        if(miniMapCamera == null) return;
        pos = GameManager.Instance.NetworkManager.LocalController.ControlledPlayer.transform.localPosition;
        rot = GameManager.Instance.NetworkManager.LocalController.ControlledPlayer.transform.localEulerAngles;
        miniMapCamera.transform.localPosition = new Vector3(pos.x, miniMapCamera.transform.localPosition.y, pos.z);
        miniMapCamera.transform.localEulerAngles = new Vector3(miniMapCamera.transform.localEulerAngles.x, rot.y, miniMapCamera.transform.localEulerAngles.z);
    }
}
