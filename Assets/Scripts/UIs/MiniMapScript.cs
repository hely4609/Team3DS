using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MiniMapScript : MyComponent
{
    //2CB 참고.
    //Layer설정으로관리
    //MiniMapCamera의 CullingMask로 보고싶은 Layer조정가능
    //RenderTexture필요
    [SerializeField] Camera miniMapCamera;
    [SerializeField] Camera largeMapCamera;
    Player controlledPlayer;
    Vector3 pos;
    Vector3 rot;
    [SerializeField] Slider minimapSlider;
    bool isMiniMapLocked = true;
    [SerializeField] GameObject largeMap;
    public bool isLargeMapOpend;

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

    bool lastCursorLock;
    public void LargeMapToggle()
    {
        if (largeMap.activeSelf)
        {
            Cursor.lockState = lastCursorLock ? CursorLockMode.Locked : CursorLockMode.None;
        }
        else
        {
            if (Cursor.lockState == CursorLockMode.Locked) lastCursorLock = true;
            else lastCursorLock = false;
            Cursor.lockState = CursorLockMode.None;
        }
        largeMap.SetActive(!largeMap.activeSelf);
        isLargeMapOpend = largeMap.activeSelf;
    }

    public float zoomSensitivity = 0.2f;
    public void LargeMapZoom(float value)
    {
        if (!largeMap.activeSelf) return;
        if (value == 0) return;

        float bef = largeMapCamera.orthographicSize;
        largeMapCamera.orthographicSize = Mathf.Clamp(largeMapCamera.orthographicSize - value * 0.05f, 10, 100);
        float aft = largeMapCamera.orthographicSize;

        Vector3 wantVector = new Vector3(Input.mousePosition.x - Screen.width * 0.5f, 0, Input.mousePosition.y - Screen.height * 0.5f);
        wantVector.Normalize();
        if (bef > aft)
        {
            largeMapCamera.transform.position += wantVector * zoomSensitivity;
            largeMapCamera.transform.position = new Vector3(Mathf.Clamp(largeMapCamera.transform.position.x + wantVector.x * 10, -100, 100), largeMapCamera.transform.position.y, Mathf.Clamp(largeMapCamera.transform.position.z + wantVector.z * 10, -100, 100));
        }
        largeMapCamera.transform.position = new Vector3(Mathf.Clamp(largeMapCamera.transform.position.x + wantVector.x * zoomSensitivity, largeMapCamera.orthographicSize - 125, -(largeMapCamera.orthographicSize - 125)), 
            largeMapCamera.transform.position.y,
            Mathf.Clamp(largeMapCamera.transform.position.z + wantVector.z * zoomSensitivity, largeMapCamera.orthographicSize - 125, -(largeMapCamera.orthographicSize - 125)));
    }

    public float dragSensitivity = 0.0055f;
    public void LargeMapDrag(Vector2 mouseDelta)
    {
        Vector3 wantVector = new(-mouseDelta.x, 0, -mouseDelta.y);
        //largeMapCamera.transform.position += largeMapCamera.orthographicSize * dragSensitivity * wantVector;
        largeMapCamera.transform.position = new Vector3(Mathf.Clamp(largeMapCamera.transform.position.x + largeMapCamera.orthographicSize * wantVector.x * dragSensitivity * 0.3f, largeMapCamera.orthographicSize - 125, -(largeMapCamera.orthographicSize - 125)),
            largeMapCamera.transform.position.y,
            Mathf.Clamp(largeMapCamera.transform.position.z + largeMapCamera.orthographicSize * wantVector.z * dragSensitivity * 0.3f, largeMapCamera.orthographicSize - 125, -(largeMapCamera.orthographicSize - 125)));
    }

    public void ShowTowerRangeToggle()
    {
        Debug.Log(miniMapCamera.cullingMask);
        Debug.Log(LayerMask.GetMask("AttackRangeMarker"));
        if((miniMapCamera.cullingMask & LayerMask.GetMask("AttackRangeMarker")) == 1)
        {
            miniMapCamera.cullingMask -= LayerMask.GetMask("AttackRangeMarker");
        }
        else
        {
            miniMapCamera.cullingMask += LayerMask.GetMask("AttackRangeMarker");
        }
    }
}
