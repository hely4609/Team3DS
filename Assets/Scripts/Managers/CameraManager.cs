using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : Manager
{
    protected Player observingPlayer;
    protected Camera mainCamera;

    public override IEnumerator Initiate()
    {
        GameManager.ManagerStarts += ManagerStart;
        return base.Initiate();
    }

    // OnSceenLoad << 씬이 로드 될때마다 메인카메라를 새로 받아와줘야한다.

    public override void ManagerStart()
    {
        mainCamera = Camera.main;
        base.ManagerStart();
    }

    public override void ManagerUpdate(float deltaTime)
    {
        // 인게임중에만 LocalPlayer의 CameraOffset을 쫓아가야함.
        if (observingPlayer == null) 
        {
            GameObject inst = GameObject.Find("LocalController");
            if (inst != null)
            {
                LocalController controller = inst.GetComponent<LocalController>();
                observingPlayer = controller.ControlledPlayer;
            }
        }        
        mainCamera.transform.position = observingPlayer.CameraOffset.position;
        mainCamera.transform.rotation = observingPlayer.CameraOffset.rotation;
    }
}
