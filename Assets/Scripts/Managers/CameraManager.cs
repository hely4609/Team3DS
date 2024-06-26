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

    // OnSceenLoad << ���� �ε� �ɶ����� ����ī�޶� ���� �޾ƿ�����Ѵ�.

    public override void ManagerStart()
    {
        mainCamera = Camera.main;
        base.ManagerStart();
    }

    public override void ManagerUpdate(float deltaTime)
    {
        // �ΰ����߿��� LocalPlayer�� CameraOffset�� �Ѿư�����.
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
