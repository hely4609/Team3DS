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
        DoDesignBuilding?.Invoke(BuildingEnum.Tower);
    }
    protected void OnBulid() { }
    protected void OnRepair() { }
    ////////////////////////////////////////////
    //protected void OnInteract() { }
}
