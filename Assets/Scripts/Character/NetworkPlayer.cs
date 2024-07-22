using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : Player
{
    // Network Character Controller 컴포넌트를 플레이어에 삽입 해줄 것! 
    [Networked] public NetworkButtons ButtonsPrevious { get; set; }

    protected override void MyUpdate(float deltaTime)
    {

    }

    public override void FixedUpdateNetwork()
    {
        /*
        if (HasInputAuthority == false)
            return;

        // Enter key is used for locking/unlocking cursor in game view.
        var keyboard = Keyboard.current;
        if (keyboard != null && (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        */
        if (GetInput(out NetworkInputData data))
        {
            /*
            data.direction = moveDir;
            data.lookRotationDelta = new Vector2(rotate_x, rotate_y);
            DoMove(data.direction);
            */
            
            // compute pressed/released state
            var pressed = data.buttons.GetPressed(ButtonsPrevious);
            var released = data.buttons.GetReleased(ButtonsPrevious);

            // store latest input as 'previous' state we had
            ButtonsPrevious = data.buttons;

            // movement (check for down)
            var vector = default(Vector3);
            if (data.buttons.IsSet(MyButtons.Forward)) { vector.z += 1; }
            if (data.buttons.IsSet(MyButtons.Backward)) { vector.z -= 1; }

            if (data.buttons.IsSet(MyButtons.Left)) { vector.x -= 1; }
            if (data.buttons.IsSet(MyButtons.Right)) { vector.x += 1; }
            //DoMove(vector.normalized * 0.1f);
            DoMove(vector);

            
            transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);
            if (cameraOffset == null)
            {
                cameraOffset = transform.Find("CameraOffset");
            }
            else cameraOffset.localEulerAngles = new Vector3(rotate_x, 0f, 0f);
            
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

    public override void ScreenRotate(Vector2 mouseDelta)
    {
        rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;

        mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
        rotate_x = rotate_x + mouseDelta_y;
        rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);
    }


}