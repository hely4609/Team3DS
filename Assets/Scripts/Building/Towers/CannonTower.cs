using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTower : Tower
{
    [SerializeField]protected float splashRadius;

    
    protected override void Initialize()
    {
        type = BuildingEnum.Tower;
        isNeedLine = true;
        TurnOnOff(false);
        objectName = "캐논 타워";

        AttackRangeSetting();
    }

    protected override void OnHit()
    {
        RaycastHit[] targets = Physics.SphereCastAll(target.transform.position, splashRadius, Vector3.up);
        foreach (RaycastHit targetRay in targets)
        {
            if(targetRay.collider.TryGetComponent<Monster>(out Monster targetData))
            {
                targetData.TakeDamage(this, attackDamage);
            }
        }
        Debug.Log($"{gameObject.name}");
    }


    public override void LockOn()
    {
        base.LockOn();
    }

}
