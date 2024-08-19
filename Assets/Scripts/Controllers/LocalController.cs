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
            // 호스트는 OnDesignBuilding을 한다.
            if(HasStateAuthority)
            {
                if(data.buttons.IsSet(MyButtons.Build)) OnBuild();
                OnDesignBuilding(data.selectedBuildingIndex);

            }
            
            // 로컬은 UI를 끈다
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

    // 로컬 인풋
    void Update()
    {
        
        // KeyCode.Return이 Enter임
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

        // 어떤 건물을 지을지 UI를 띄워준다.
        // 그리고 그 버튼을 누르면 거기서 플레이어의 건물짓기를 시도한다.
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
                // 플레이어가 지금 자기가 하고있는 상호작용이 뭔지 알아야함.
                // 업데이트 함수를 등록해서 뗄때까지 실행
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
