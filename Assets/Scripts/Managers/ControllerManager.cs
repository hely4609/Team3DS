using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : Manager
{
    protected Player controlledPlayer;

    protected void OnMove(InputValue value) 
    {
        
    }
    //protected void OnScreenRotate(InputValue value)
    //{
    //    controlledPlayer.ScreenRotate(value.Get<Vector2>());
    //}

    public override IEnumerator Initiate() { yield return null; }

    protected void OnMove(InputValue value) { }

    protected void OnPickUp() { }
    protected void OnPutDown() { }
    protected void OnDesignBuiling() { }
    protected void OnBulid() { }
    protected void OnRepair() { }
    ////////////////////////////////////////////
    //protected void OnInteract() { }
}
