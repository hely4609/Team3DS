using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private float rotate_x;
    private float mouseDelta_y;

    private void ScreenRotate(Vector2 mouseDelta)
    {
        mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
        rotate_x = rotate_x + mouseDelta_y;
        rotate_x = Mathf.Clamp(rotate_x, -45f, 45f); // 위, 아래 고정
        transform.localEulerAngles = new Vector3(rotate_x, 0f, 0f);
    }

    protected void OnScreenRotate(InputValue value)
    {
        ScreenRotate(value.Get<Vector2>());
    }

}
