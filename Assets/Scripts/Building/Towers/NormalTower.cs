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
        name = "�⺻ Ÿ��";

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
