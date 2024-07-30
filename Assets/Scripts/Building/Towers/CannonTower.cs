using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTower : Tower
{
    protected float splashRadius;
    protected override void Initialize()
    {
        type = BuildingEnum.Tower;
        isNeedLine = true;
        AttackDamage = 1;
        AttackSpeed = 0.5f;
        AttackRange = 1;
        buildingTimeMax = 10;
        splashRadius = 10;
        size = new Vector2Int(4, 4);
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

    public override void BuildBuilding(float deltaTime)
    {
        // 마우스를 누르고 있으면 점점 수치가 차오름.
        // 델타 타임 만큼 자신의 buildingTimeCurrent를 올림.
        if (CompletePercent < 1)
        {
            buildingTimeCurrent += deltaTime;
        }
        else
        {

        }

        // 마우스를 떼면 정지. 다른 곳으로 돌려도 정지.

        // 완성되면 완성본 Material로 한다.

        // 건설 완료시 
        foreach (MeshRenderer r in meshes)
        {
            r.material.SetFloat("_CompletePercent", CompletePercent);
        }

        if (CompletePercent >= 1)
        {
            foreach (MeshRenderer r in meshes)
                r.material = ResourceManager.Get(ResourceEnum.Material.ION_Cannon);

        }
    }

    public override void LockOn()
    {
        base.LockOn();
    }

    public override string GetName()
    {
        return "ConnonTower";
    }
    
}
