using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTower : Tower
{
    protected override void Initialize()
    {
        // ����Ʈ ��.
        type = BuildingEnum.Tower;
        isNeedLine = true;
        TurnOnOff(false);
        objectName = "�⺻ Ÿ��";

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
