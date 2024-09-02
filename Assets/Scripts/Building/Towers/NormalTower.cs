using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTower : Tower
{
    protected override void Initialize()
    {
        // 디폴트 값.
        type = BuildingEnum.Tower;
        isNeedLine = true;
        TurnOnOff(false);
        objectName = "기본 타워";

        powerConsumption = 10;

        AttackRangeSetting();
    }
    protected override void OnHit()
    {
        base.OnHit();
    }
    public override void LockOn()
    {
        base.LockOn();
    }
}
