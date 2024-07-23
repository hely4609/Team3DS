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
    protected void OnDesignBuiling() 
    {
        // 어떤 건물을 지을지 UI를 띄워준다.
        // 그리고 그 버튼을 누르면 거기서 플레이어의 건물짓기를 시도한다.
        DoDesignBuilding?.Invoke(ResourceEnum.Prefab.Turret1a);
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
    protected void OnInteraction() 
    {
        DoInteraction?.Invoke(ControlledPlayer.InteractionObject);
    }

    protected void OnTest(InputValue value)
    {
        if (ControlledPlayer.InteractionObject == null) return;

        if (value.isPressed)
        {
            //Debug.Log("눌렀다");
            // 플레이어가 지금 자기가 하고있는 상호작용이 뭔지 알아야함.
            ControlledPlayer.InteractionObject.InteractionStart(controlledPlayer);
            ControlledPlayer.isInteracting = true;
            // 업데이트 함수를 등록해서 뗄때까지 실행
        }
        else
        {
            //Debug.Log("뗏다");
            ControlledPlayer.InteractionObject.InteractionEnd();
            ControlledPlayer.isInteracting= false;
        }    
    }
}
