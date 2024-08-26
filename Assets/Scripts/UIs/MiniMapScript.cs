using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapScript : MyComponent
{
    //2CB 참고.
    //Layer설정으로관리
    //MiniMapCamera의 CullingMask로 보고싶은 Layer조정가능
    //RenderTexture필요
    Camera miniMapCamera;
    Player controlledPlayer;
    Vector3 pos;
    Vector3 rot;
    bool isMiniMapLocked = true;

    protected override void MyStart()
    {
        miniMapCamera = GetComponentInChildren<Camera>();
        controlledPlayer = GameManager.Instance.NetworkManager.LocalController.ControlledPlayer;
    }
    protected override void MyUpdate(float deltaTime)
    {
        if (miniMapCamera == null || !GameManager.IsGameStart) return;
        pos = controlledPlayer.transform.localPosition;
        miniMapCamera.transform.localPosition = new Vector3(pos.x, miniMapCamera.transform.localPosition.y, pos.z);
        if (!isMiniMapLocked)
        {
            rot = controlledPlayer.transform.localEulerAngles;
            miniMapCamera.transform.localEulerAngles = new Vector3(miniMapCamera.transform.localEulerAngles.x, rot.y, miniMapCamera.transform.localEulerAngles.z);
        }
    }

    public void MiniMapLockToggle()
    {
        if (isMiniMapLocked)
        {
            isMiniMapLocked = false;
        }
        else
        {
            isMiniMapLocked = true;
            miniMapCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
        }
    }

    public void ZoomIn()
    {
        miniMapCamera.orthographicSize = Mathf.Clamp(miniMapCamera.orthographicSize - 5, 10, miniMapCamera.orthographicSize);
    }

    public void ZoomOut()
    {
        miniMapCamera.orthographicSize = Mathf.Clamp(miniMapCamera.orthographicSize + 5, miniMapCamera.orthographicSize, 100);
    }
}
