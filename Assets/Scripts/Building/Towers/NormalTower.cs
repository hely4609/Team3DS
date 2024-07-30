using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTower : Tower
{
    protected override void Initialize()
    {
        type = BuildingEnum.Tower;
        AttackDamage = 1;
        AttackSpeed = 1;
        AttackRange = 50;
        buildingTimeMax = 10;
        size = new Vector2Int(10, 10);
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
