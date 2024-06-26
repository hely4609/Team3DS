using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{    
    protected ControllerManager possessionController;
    protected Transform cameraOffset;
    public Transform CameraOffset => cameraOffset;

    protected GameObject bePicked;
    // protected bool isHandFree;
    protected Building designingBuilding;

    protected float rotate_x; // 마우스 이동에 따른 시점 회전 x값
    protected float rotate_y; // 마우스 이동에 따른 시점 회전 y값
    protected float mouseDelta_y; // 마우스 이동 변화량 y값

    protected Vector3 moveDir;

    protected override void MyStart()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    protected override void MyUpdate(float deltaTime)
    {
        if (moveDir.magnitude == 0)
        {
            float velocityX = Mathf.Lerp(rb.velocity.x, 0f, 0.1f);
            float velocityZ = Mathf.Lerp(rb.velocity.z, 0f, 0.1f);
            rb.velocity = new Vector3(velocityX, 0f, velocityZ);
        }
        else
        {
            rb.velocity = (transform.forward * moveDir.z + transform.right * moveDir.x).normalized * moveSpeed;
        }
    }

    public override void Move(Vector3 direction)
    {
        moveDir = direction.normalized;
    }

    public void ScreenRotate(Vector2 mouseDelta)
    {
        rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;
        transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

        mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
        rotate_x = rotate_x + mouseDelta_y;
        rotate_x = Mathf.Clamp(rotate_x, -45f, 45f);
        if (cameraOffset == null)
        {
            cameraOffset = transform.Find("CameraOffset");
        }
        cameraOffset.localEulerAngles = new Vector3(rotate_x, 0f, 0f);
    }
    public bool PickUp(GameObject target) { return default; }
    public bool PutDown() { return default; }
    public bool DesignBuiling(BuildingEnum wantBuilding) { return default; }
    public bool Build(Building building) { return default; }
    public bool Repair(EnergyBarrierGenerator target) { return default; }
}
