using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapScript : MyComponent
{
    //2CB ����.
    //Layer�������ΰ���
    //MiniMapCamera�� CullingMask�� ������� Layer��������
    //RenderTexture�ʿ�
    [SerializeField] Camera miniMapCamera;
    [SerializeField] Camera largeMapCamera;
    Player controlledPlayer;
    Vector3 pos;
    Vector3 rot;
    [SerializeField] Slider minimapSlider;
    bool isMiniMapLocked = true;
    [SerializeField] GameObject largeMap;

    protected override void MyStart()
    {
        controlledPlayer = GameManager.Instance.NetworkManager.LocalController.ControlledPlayer;
        minimapSlider.value = miniMapCamera.orthographicSize;
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
        minimapSlider.value = miniMapCamera.orthographicSize;
    }

    public void ZoomOut()
    {
        miniMapCamera.orthographicSize = Mathf.Clamp(miniMapCamera.orthographicSize + 5, miniMapCamera.orthographicSize, 100);
        minimapSlider.value = miniMapCamera.orthographicSize;
    }

    public void ZoomSlider()
    {
        miniMapCamera.orthographicSize = minimapSlider.value;
    }

    public void LargeMapToggle()
    {
        largeMap.SetActive(!largeMap.activeSelf);
    }

    public void LargeMapZoom(float value)
    {
        if (!largeMap.activeSelf) return;
        largeMapCamera.orthographicSize = Mathf.Clamp(largeMapCamera.orthographicSize - value * 0.05f, 10, 100);
    }
}
