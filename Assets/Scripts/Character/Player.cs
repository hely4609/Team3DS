using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    protected GameObject bePicked; // �����÷��� ������Ʈ
    // protected bool isHandFree;
    protected Building designingBuilding;

    protected bool PickUp(GameObject target) { return default; }
    protected bool PutDown() { return default; }

// ���� �ǹ��� ���� ���ٴϴ� �޼���
protected bool DesignBuiling(BuildingEnum wantBuilding) { return default; }
    // ������ ��ġ�� �ε���� �Ǽ��ϴ� �޼���
    protected bool Build(Building building) { return default; }

    protected bool Repair(EnergyBarrierGenerator target) { return default; }
}
