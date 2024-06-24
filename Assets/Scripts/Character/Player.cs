using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// �׽�Ʈ
using UnityEngine.InputSystem;

public class Player : Character
{
    protected GameObject bePicked; // �����÷��� ������Ʈ
    // protected bool isHandFree;
    protected Building designingBuilding;

    private float rotate_y;

    public void ScreenRotate(Vector2 mouseDelta)
    {
        rotate_y = transform.eulerAngles.y + mouseDelta.x * 0.02f * 10f;

        transform.localEulerAngles = new Vector3(0f, rotate_y, 0f);

        //Debug.Log($"{mouseDelta.x}, {mouseDelta.y}");
        // transform.Rotate(0f, mouseDelta.x * 0.02f * 10f, 0f);
    }
    public bool PickUp(GameObject target) { return default; }
    public bool PutDown() { return default; }

    // ���� �ǹ��� ���� ���ٴϴ� �޼���
    public bool DesignBuiling(BuildingEnum wantBuilding) { return default; }
    // ������ ��ġ�� �ε���� �Ǽ��ϴ� �޼���
    public bool Build(Building building) { return default; }

    public bool Repair(EnergyBarrierGenerator target) { return default; }

    // �׽�Ʈ
    protected void OnScreenRotate(InputValue value)
    {
        // ���콺�� ��ȭ���� �޾Ƽ� Vector2�� �Ѱ��ش�.
        ScreenRotate(value.Get<Vector2>());       
    }
}
