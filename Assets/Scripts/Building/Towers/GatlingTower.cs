using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingTower : Tower
{
    protected override void Initialize()
    {
        cost = 20;

        // 디폴트 값.
        buildingType = BuildingEnum.Tower;
        isNeedLine = true;
        TurnOnOff(false);
        objectName = "Gatling Tower";
        powerConsumption = 40;

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
