using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalController : ControllerBase
{
    protected void OnMove(InputValue value)
    {
        // 테스트
        //if (controlledPlayer == null)
        //{
        //    controlledPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //}
        //
        //controlledPlayer.Move(value.Get<Vector3>());

        DoMove?.Invoke(value.Get<Vector3>());
    }

    protected void OnScreenRotate(InputValue value)
    {
        // 테스트
        //if (controlledPlayer == null)
        //{
        //    controlledPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //}
        ////
        //controlledPlayer.ScreenRotate(value.Get<Vector2>());

        DoScreenRotate?.Invoke(value.Get<Vector2>());
    }

    protected void OnPickUp() { }
    protected void OnPutDown() { }
    protected void OnDesignBuiling() { }
    protected void OnBulid() { }
    protected void OnRepair() { }
    ////////////////////////////////////////////
    //protected void OnInteract() { }
}
