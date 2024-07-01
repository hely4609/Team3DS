using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Player : Character
{
    protected ControllerBase possessionController;
    protected Transform cameraOffset;
    public Transform CameraOffset => cameraOffset;

    protected GameObject bePicked;
    // protected bool isHandFree;
    protected Building designingBuilding;

    protected float rotate_x; // 마우스 이동에 따른 시점 회전 x값
    protected float rotate_y; // 마우스 이동에 따른 시점 회전 y값
    protected float mouseDelta_y; // 마우스 이동 변화량 y값

    protected Vector3 moveDir;
    protected Vector3 currentDir = Vector3.zero;

    public bool TryPossession() => possessionController == null;

    protected void RegistrationFunctions(ControllerBase targetController)
    {
        targetController.DoMove -= Move;
        targetController.DoScreenRotate -= ScreenRotate;

        targetController.DoMove += Move;
        targetController.DoScreenRotate += ScreenRotate;
    }
    protected void UnRegistrationFunction(ControllerBase targetController)
    {
        targetController.DoMove -= Move;
        targetController.DoScreenRotate -= ScreenRotate;
    }

    public virtual void Possession(ControllerBase targetController)
    {
        if (TryPossession() == false)
        {
            targetController.OnPossessionFailed(this);
            return;
        }

        possessionController = targetController;

        //함수를 전달만 하면 되는 거예요! -> 컨트롤러가 일하고 싶을 때
        //내 함수를 대신 실행해줘!
        RegistrationFunctions(possessionController);
        targetController.OnPossessionComplete(this);
    }
    public virtual void UnPossess()
    {
        if (possessionController == null) return;
        UnRegistrationFunction(possessionController);
        possessionController.OnUnPossessionComplete(this);
        possessionController = null;
    }

    protected override void MyStart()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (cameraOffset == null)
        {
            cameraOffset = transform.Find("CameraOffset");
        }
    }

    

    protected override void MyUpdate(float deltaTime)
    {
        // 테스트
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        //
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

        AnimFloat?.Invoke("Speed", rb.velocity.magnitude);

        currentDir = new Vector3(Mathf.Lerp(currentDir.x, moveDir.x, 0.1f), currentDir.y, Mathf.Lerp(currentDir.z, moveDir.z, 0.1f));

        AnimFloat?.Invoke("MoveForward", currentDir.z);
        AnimFloat?.Invoke("MoveRight", currentDir.x);
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
