using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class NetworkPhotonCallbacks
{
    Vector3 moveDir;
    float rotate_x, rotate_y, mouseDelta_y;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        data.direction = moveDir;
        data.lookRotationDelta = new Vector2(rotate_x, rotate_y);
 

        input.Set(data);
    }
    public void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector3>();
    }
    public void OnScreenRotate(InputValue value)
    {
        // rotate_x가 위아래이고 y가 좌우임
        Vector2 mouseDelta = value.Get<Vector2>();
        rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;
        transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

        mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
        rotate_x = rotate_x + mouseDelta_y;
        rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
}
