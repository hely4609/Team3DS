using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingTower : Tower
{
    protected override void Initialize()
    {
        // 디폴트 값.
        buildingType = BuildingEnum.Tower;
        isNeedLine = true;
        TurnOnOff(false);
        objectName = "Gatling Tower";

        AttackRangeSetting();
        maxRopeLength = 10;
        currentRopeLength = maxRopeLength;
        base.Initialize();
    }
    public override void Attack()
    {
        if (!target)
        {
            attackAnimator.SetBool("Attack", false);
            LockOn();

        }
        else
        {
            attackAnimator.SetBool("Attack", true);

            gunBarrel.transform.LookAt(target.transform);
            gunBarrel.transform.eulerAngles = new Vector3(0, gunBarrel.transform.eulerAngles.y + gunBarrelRotateCorrection, 0);
            if (nowTime <= 0)
            {
                OnHit();
                nowTime = attackSpeed;
            }
            else
            {
                nowTime -= Time.deltaTime;
            }
        }
    }

    protected override void OnHit()
    {
        if (!target.isReady)
        {
            MonsterListOut(target);
            target = null;
            Attack();
            return;
        }
        else
        {
            target.TakeDamage(this, attackDamage);
        }
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
