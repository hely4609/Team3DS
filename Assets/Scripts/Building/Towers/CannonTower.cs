using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTower : Tower
{
    [SerializeField]protected float splashRadius;

    
    protected override void Initialize()
    {
        buildingType = BuildingEnum.Tower;
        isNeedLine = true;
        TurnOnOff(false);
        objectName = "Cannon Tower";

        AttackRangeSetting();
        maxRopeLength = 10;
        currentRopeLength = maxRopeLength;

        base.Initialize();
    }

    protected override void OnHit()
    {
        RaycastHit[] targets = Physics.SphereCastAll(target.transform.position, splashRadius, Vector3.up);
        if(targets.Length > 0) attackAnimator.SetBool("Attack", true);
        foreach (RaycastHit targetRay in targets)
        {
            if(targetRay.collider.TryGetComponent<Monster>(out Monster targetData))
            {
                if(targetData == target)
                {
                    targetData.TakeDamage(this, attackDamage, "Explosion");
                }
                else
                {
                    targetData.TakeDamage(this, attackDamage, "");
                }
            }
        }
        Debug.Log($"{gameObject.name}");
    }


    public override void LockOn()
    {
        base.LockOn();
    }
    public override void Upgrade()
    {
        base.Upgrade();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (target != null) Gizmos.DrawWireSphere(target.transform.position, splashRadius);
    }
}
