using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : Manager
{
    protected Player controlledPlayer;

    public override IEnumerator Initiate() { return default; }

    protected void OnMove(InputValue value) { }
    protected void OnPickUp() { }
    protected void OnPutDown() { }
    protected void OnDesignBuiling() { }
    protected void OnBulid() { }
    protected void OnRepair() { }
    ////////////////////////////////////////////
    //protected void OnInteract() { }
}
