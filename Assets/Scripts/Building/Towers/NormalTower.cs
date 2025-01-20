using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTower : Tower
{
    protected override void Initialize()
    {
        cost = 10;
        // ����Ʈ ��.
        buildingType = BuildingEnum.Tower;
        isNeedLine = true;
        TurnOnOff(false);
        objectName = "Normal Tower";
        powerConsumption = 10;

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
        if (!HasStateAuthority) return;
        if (UpgradeRequire > GameManager.Instance.BuildingManager.supply.TotalOreAmount)
        {
            Debug.Log("���׷��̵忡 �ʿ��� ������ �����մϴ�.");
            return;
        }
        attackDamage += 2;
        GameManager.Instance.BuildingManager.supply.TotalOreAmount -= UpgradeRequire;
        TotalUpgradeCost += UpgradeRequire;
        UpgradeRequire = Mathf.CeilToInt(UpgradeRequire * 1.2f);
        Level += 1;
    }
}
