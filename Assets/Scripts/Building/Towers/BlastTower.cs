using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastTower : Tower
{
    protected override void Initialize()
    {
        cost = 40;

        // ����Ʈ ��.
        buildingType = BuildingEnum.Tower;
        isNeedLine = true;
        TurnOnOff(false);
        objectName = "Blast Tower";
        powerConsumption = 30;

        AttackRangeSetting();
        maxRopeLength = 10;
        currentRopeLength = maxRopeLength;
        base.Initialize();
    }
    protected override void OnHit()
    {
        base.OnHit();
    }
    public override void LockOn()
    {
        base.LockOn();
    }
    public override void Upgrade()
    {
        base.Upgrade();
    }
}
