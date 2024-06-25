using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : Manager
{
    protected Player controlledPlayer;

    public override IEnumerator Initiate() 
    {
        yield return null; 
    }

    protected void OnMove(InputValue value)
    {
        // Å×½ºÆ®
        if (controlledPlayer == null)
        {
            controlledPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        controlledPlayer.Move(value.Get<Vector3>());
    }

    protected void OnScreenRotate(InputValue value)
    {
        if (controlledPlayer == null)
        {
            controlledPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        controlledPlayer.ScreenRotate(value.Get<Vector2>());
    }

    protected void OnPickUp() { }
    protected void OnPutDown() { }
    protected void OnDesignBuiling() { }
    protected void OnBulid() { }
    protected void OnRepair() { }
    ////////////////////////////////////////////
    //protected void OnInteract() { }
}
