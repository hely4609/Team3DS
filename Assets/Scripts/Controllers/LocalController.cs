using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalController : ControllerBase
{
    
    public override void FixedUpdateNetwork()
    {
        // KeyCode.Return이 Enter임
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }

        if (GetInput(out NetworkInputData data))
        {
            OnMove(data.moveDirection);
            OnScreenRotate(data.lookRotationDelta);
            OnInteraction(data.buttons.IsSet(MyButtons.Interaction));

            if (HasStateAuthority)
            {
                //HoldingDesign();
                OnDesignBuilding(data.selectedBuildingIndex);
            }
            if (data.buttons.IsSet(MyButtons.Build)) DoBuild();
        }
    }

    protected void OnMove(Vector3 direaction)
    {
        DoMove?.Invoke(direaction);
    }

    protected void OnScreenRotate(Vector2 mouseDelta)
    {
        DoScreenRotate?.Invoke(mouseDelta);
    }

    protected void OnPickUp() { }
    protected void OnPutDown() { }
    protected void OnDesignBuilding(int index) 
    {
        if (ControlledPlayer?.buildingSeletUI.activeInHierarchy == false) return;
        
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
    protected void OnInteraction(bool isPressed) 
    {
        if (controlledPlayer?.InteractionObject == null) return;

        if (isPressed)
        {
            // 플레이어가 지금 자기가 하고있는 상호작용이 뭔지 알아야함.
            // 업데이트 함수를 등록해서 뗄때까지 실행
            DoInteractionStart?.Invoke();
        }
        else
        {
            DoInteractionEnd?.Invoke();
        }
    }
}
