using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : Manager
{
    // 컨트롤러 매니저 = 로컬플레이어 지정 컨트롤러들(로컬,네트워크) 관리

    //protected Player controlledPlayer;

    protected List<ControllerBase> controllerList;

    // OnSceenLoad
    public override IEnumerator Initiate()
    {
        if (controllerList == null)
        {
            controllerList = new List<ControllerBase>();
            // Controller 목록받아올 방법 생각해보기.
        }
        else yield break;
        yield return null;
    }

    //protected void OnMove(InputValue value)
    //{
    //    // 테스트
    //    if (controlledPlayer == null)
    //    {
    //        controlledPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    //    }
    //    controlledPlayer.Move(value.Get<Vector3>());
    //}

    //protected void OnScreenRotate(InputValue value)
    //{
    //    if (controlledPlayer == null)
    //    {
    //        controlledPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    //    }
    //    controlledPlayer.ScreenRotate(value.Get<Vector2>());
    //}

    //protected void OnPickUp() { }
    //protected void OnPutDown() { }
    //protected void OnDesignBuiling() { }
    //protected void OnBulid() { }
    //protected void OnRepair() { }
    //////////////////////////////////////////////
    ////protected void OnInteract() { }
}
