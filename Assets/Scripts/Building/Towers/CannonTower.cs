using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTower : Tower
{
    protected float splashRadius;
    protected override void Initialize()
    {
        type = BuildingEnum.Tower;
        AttackDamage = 1;
        AttackSpeed = 0.5f;
        AttackRange = 5;
        buildingTimeMax = 10;
        splashRadius = 10;
        size = new Vector2Int(10, 10);
        TurnOnOff(true);
    }

    protected override void OnHit()
    {
        RaycastHit[] targets = Physics.SphereCastAll(target.transform.position, splashRadius, Vector3.up);
        foreach (RaycastHit targetRay in targets)
        {
            if(targetRay.collider.TryGetComponent<TestMonster>(out TestMonster targetData))
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
