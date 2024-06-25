using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 테스트
using UnityEngine.InputSystem;

public class Player : Character
{
    protected ControllerManager possessionController;
    protected Camera possessionCamera;

    protected GameObject bePicked; // 전선플러그 오브젝트
    // protected bool isHandFree;
    protected Building designingBuilding;
    
    private float rotate_x;
    private float rotate_y;
    private float mouseDelta_y;

    public void ScreenRotate(Vector2 mouseDelta)
    {
        rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;
        transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

        mouseDelta_y = -mouseDelta.y * 0.02f * 10f;
        rotate_x = rotate_x + mouseDelta_y;
        rotate_x = Mathf.Clamp(rotate_x, -45f, 45f); // 위, 아래 고정
        possessionCamera.transform.localEulerAngles = new Vector3(rotate_x, 0f, 0f);

        //Debug.Log($"{mouseDelta.x}, {mouseDelta.y}");
        // transform.Rotate(0f, mouseDelta.x * 0.02f * 10f, 0f);
    }
    public bool PickUp(GameObject target) { return default; }
    public bool PutDown() { return default; }

    // 투명 건물을 만들어서 들고다니는 메서드
    public bool DesignBuiling(BuildingEnum wantBuilding) { return default; }
    // 실제로 망치를 두드려서 건설하는 메서드
    public bool Build(Building building) { return default; }

    public bool Repair(EnergyBarrierGenerator target) { return default; }

    // 테스트
    //protected void OnScreenRotate(InputValue value)
    //{
    //    // 마우스의 변화량을 받아서 Vector2로 넘겨준다.
    //    ScreenRotate(value.Get<Vector2>());       
    //}
}
