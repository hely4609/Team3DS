using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalController : ControllerBase
{
    protected void OnMove(InputValue value)
    {
        DoMove?.Invoke(value.Get<Vector3>());
    }

    protected void OnScreenRotate(InputValue value)
    {
        DoScreenRotate?.Invoke(value.Get<Vector2>());
    }

    protected void OnPickUp() { }
    protected void OnPutDown() { }
    protected void OnDesignBuilding(InputValue value) 
    {
        if (ControlledPlayer.buildingSeletUI.activeInHierarchy == false) return;

        Vector3 result = value.Get<Vector3>();

        if (result.magnitude == 0f) return;

        else if (result == Vector3.up)
        {
            DoDesignBuilding?.Invoke(0);
        }
        else if (result == Vector3.down)
        {
            DoDesignBuilding?.Invoke(1);
        }
        else if (result == Vector3.left)
        {
            DoDesignBuilding?.Invoke(2);
        }
        else if (result == Vector3.right)
        {
            DoDesignBuilding?.Invoke(3);
        }
        else if (result == Vector3.forward)
        {
            DoDesignBuilding?.Invoke(4);
        }

        // 어떤 건물을 지을지 UI를 띄워준다.
        // 그리고 그 버튼을 누르면 거기서 플레이어의 건물짓기를 시도한다.
        // DoDesignBuilding?.Invoke(ResourceEnum.Prefab.Turret1a);
    }
    protected void OnBuild() 
    {
        DoBuild?.Invoke();
    }

    protected void OnMouseWheel(InputValue value)
    {
        DoMouseWheel?.Invoke(value.Get<Vector2>());
    }
    protected void OnRepair() { }
    ////////////////////////////////////////////
    protected void OnInteraction(InputValue value) 
    {
        if (ControlledPlayer.InteractionObject == null) return;

        if (value.isPressed)
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
