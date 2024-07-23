using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : Player
{
    // Network Character Controller 컴포넌트를 플레이어에 삽입 해줄 것! 
    [Networked] public NetworkButtons ButtonsPrevious { get; set; }

    protected override void MyStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    protected override void MyUpdate(float deltaTime)
    {

    }

    public override void FixedUpdateNetwork()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }
        if (GetInput(out NetworkInputData data))
        {
            DoMove(data.direction);
            DoScreenRotate(data.lookRotationDelta);

        }
    }

    public void DoMove(Vector3 direction)
    {
        transform.position += (transform.forward * direction.z + transform.right * direction.x).normalized * 0.1f;
        AnimFloat?.Invoke("Speed", direction.magnitude);

        //currentDir = new Vector3(Mathf.Lerp(currentDir.x, moveDir.x, 0.1f), currentDir.y, Mathf.Lerp(currentDir.z, moveDir.z, 0.1f));

        AnimFloat?.Invoke("MoveForward", direction.z);
        AnimFloat?.Invoke("MoveRight", direction.x);
    }

    public void DoScreenRotate(Vector2 mouseDelta)
    {
        transform.localEulerAngles = new Vector3(0f, mouseDelta.y, 0f);
        if (cameraOffset == null)
        {
            cameraOffset = transform.Find("CameraOffset");
        }
        cameraOffset.localEulerAngles = new Vector3(mouseDelta.x, 0f, 0f);


    }


}