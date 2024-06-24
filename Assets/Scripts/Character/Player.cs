using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    protected GameObject bePicked; // 전선플러그 오브젝트
    // protected bool isHandFree;
    protected Building designingBuilding;

    protected bool PickUp(GameObject target) { return default; }
    protected bool PutDown() { return default; }

// 투명 건물을 만들어서 들고다니는 메서드
protected bool DesignBuiling(BuildingEnum wantBuilding) { return default; }
    // 실제로 망치를 두드려서 건설하는 메서드
    protected bool Build(Building building) { return default; }

    protected bool Repair(EnergyBarrierGenerator target) { return default; }
}
