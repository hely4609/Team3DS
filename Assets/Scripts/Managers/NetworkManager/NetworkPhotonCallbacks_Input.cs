using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class NetworkPhotonCallbacks
{
    Vector3 moveDir;
    Vector2 mouseDelta;
    float rotate_x, rotate_y, mouseDelta_y;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        data.direction = moveDir;
        data.lookRotationDelta = mouseDelta;
 

        input.Set(data);
    }
    public void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector3>();
    }
    public void OnScreenRotate(InputValue value)
    {
        mouseDelta = value.Get<Vector2>();
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
}
