using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
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
            if(HasStateAuthority)
            {
                OnFarming(data.buttons.IsSet(MyButtons.Farming));
                controlledPlayer.NetworkIsFarmingPressed = data.buttons.IsSet(MyButtons.Farming);

            }

            OnMouseWheel(data.scrollbarDelta);

            if(data.buttons.IsSet(MyButtons.Cancel)) OnCancel();
            if (data.buttons.IsSet(MyButtons.Rope)) OnRope();

            //if (data.buttons.IsSet(MyButtons.Farming)) OnFarming();

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
                if(HasInputAuthority && controlledPlayer.IsThisPlayerCharacterUICanvasActivated)
                {
                    controlledPlayer.buildingSelectUI.SetActive(false);
                    //controlledPlayer.buildingConfirmUI.SetActive(true);
                }
                controlledPlayer.IsThisPlayerCharacterUICanvasActivated = false;
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
            if (controlledPlayer.DesigningBuilding != null)
            runner.Despawn(controlledPlayer.DesigningBuilding.GetComponent<NetworkObject>());
            if (controlledPlayer.ropeBuilding != null)
            controlledPlayer.ropeBuilding.ResetRope(controlledPlayer, MyNumber);
            runner.Despawn(controlledPlayer.GetComponent<NetworkObject>());
            NetworkPhotonCallbacks.playerArray[MyNumber] = false;
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
        {
            if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }
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
    protected void OnRope()
    {
        
    }

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
        if (isFarmingKeyPressed) return;

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

    protected void OnMouseWheel(Vector2 value)
    {
        DoMouseWheel?.Invoke(value);
        if (minimap != null) minimap.LargeMapZoom(value.y);
    }

    protected void OnMouseWheel(InputValue value)
    {
        return;
    }
    protected void OnRepair() { }
    ////////////////////////////////////////////

    bool alreadyPressed = false;
    protected void OnInteraction(bool isPressed) 
    {
        if (controlledPlayer == null) return;

        if (isFarmingKeyPressed) return;

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

    protected void OnCancel()
    {
        DoCancel?.Invoke();
    }

    bool isFarmingKeyPressed = false;
    protected void OnFarming(bool isPressed)
    {
        if (controlledPlayer?.DesigningBuilding != null) return;
        if (alreadyPressed) return;

        if (isPressed ^ isFarmingKeyPressed)
        {
            if (isPressed) isFarmingKeyPressed = true;
            else           isFarmingKeyPressed = false;
          
            DoFarming?.Invoke(isFarmingKeyPressed);
        }
    }

    protected void OnKeyGuide()
    {
        DoKeyGuide?.Invoke();
    }

    MiniMapScript minimap;
    protected void OnLargeMap()
    {
        if(minimap == null)
        {
            minimap = FindObjectOfType<MiniMapScript>();
        }
        if(minimap != null) minimap.LargeMapToggle();
    }
}
