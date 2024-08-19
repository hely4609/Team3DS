using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalController : ControllerBase
{
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            OnMove(data.moveDirection);
            //OnScreenRotate(data.lookRotationDelta);
            OnInteraction(data.buttons.IsSet(MyButtons.Interaction));

            // DesignBuilding
            // ȣ��Ʈ�� OnDesignBuilding�� �Ѵ�.
            if(HasStateAuthority)
            {
                if(data.buttons.IsSet(MyButtons.Build)) OnBuild();
                OnDesignBuilding(data.selectedBuildingIndex);

            }
            
            // ������ UI�� ����
            if(data.selectedBuildingIndex != -1)
            {
                controlledPlayer.IsThisPlayerCharacterUICanvasActivated = false;
                Debug.Log($"IsThisPCUICA : {controlledPlayer.IsThisPlayerCharacterUICanvasActivated}");
                if(HasInputAuthority)
                {
                    controlledPlayer.buildingSelectUI.SetActive(false);

                }
            }
        }
    }
    protected override void MyStart()
    {

    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if(HasStateAuthority)
        {
            runner.Despawn(controlledPlayer.GetComponent<NetworkObject>());
        }
    }

    // ���� ��ǲ
    void Update()
    {
        
        // KeyCode.Return�� Enter��
        // OnCursorLockTogle
        //if (Input.GetKeyDown(KeyCode.Return) && HasInputAuthority)
        //{
        //    if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
        //    else Cursor.lockState = CursorLockMode.Locked;
        //}

        // OnOpenDesignBuildingUI
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    Debug.Log($"DesiningBuilding : {controlledPlayer.DesigningBuilding}");
        //    if (controlledPlayer.DesigningBuilding == null && myAuthority == Runner.LocalPlayer)
        //        controlledPlayer.buildingSelectUI.SetActive(true);
        //}
    }

    protected void OnCursorLockTogle()
    {
        if (HasInputAuthority)

        if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }

    protected void OnOpenDesignBuildingUI()
    {
        if (controlledPlayer.DesigningBuilding == null && HasInputAuthority)
            controlledPlayer.buildingSelectUI.SetActive(true);
    }

    protected void OnMove(Vector3 direaction)
    {
        DoMove?.Invoke(direaction);
    }

    //protected void OnMove(InputValue value)
    //{
    //    DoMove?.Invoke(value.Get<Vector3>());
    //}

    protected void OnScreenRotate(Vector2 mouseDelta)
    {
        
        DoScreenRotate?.Invoke(mouseDelta);
    }

    protected void OnScreenRotate(InputValue value)
    {
        DoScreenRotate?.Invoke(value.Get<Vector2>());
    }

    protected void OnPickUp() { }
    protected void OnPutDown() { }
    protected void OnDesignBuilding(int index) 
    {
        if (controlledPlayer == null || controlledPlayer.DesigningBuilding != null || !controlledPlayer.IsThisPlayerCharacterUICanvasActivated) return;
        DoDesignBuilding?.Invoke(index);

        // � �ǹ��� ������ UI�� ����ش�.
        // �׸��� �� ��ư�� ������ �ű⼭ �÷��̾��� �ǹ����⸦ �õ��Ѵ�.
        // DoDesignBuilding?.Invoke(ResourceEnum.Prefab.Turret1a);
    }
    protected void OnBuild() 
    {
        DoBuild?.Invoke();
    }

    protected void OnMouseWheel(Vector2 scrollDelta)
    {
        DoMouseWheel?.Invoke(scrollDelta);
    }
    protected void OnRepair() { }
    ////////////////////////////////////////////

    bool alreadyPressed = false;
    protected void OnInteraction(bool isPressed) 
    {
        if (controlledPlayer == null) return;

        if (isPressed ^ alreadyPressed)
        { 
            if(isPressed)
            {
                // �÷��̾ ���� �ڱⰡ �ϰ��ִ� ��ȣ�ۿ��� ���� �˾ƾ���.
                // ������Ʈ �Լ��� ����ؼ� �������� ����
                DoInteractionStart?.Invoke();
                alreadyPressed = true;
            }
            else
            {
                DoInteractionEnd?.Invoke();
                alreadyPressed = false;
            }
        
        }
    }
}
