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
        objectName = "ĳ�� Ÿ��";

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

    public override void BuildBuilding(float deltaTime)
    {
        // ���콺�� ������ ������ ���� ��ġ�� ������.
        // ��Ÿ Ÿ�� ��ŭ �ڽ��� buildingTimeCurrent�� �ø�.
        if (CompletePercent < 1)
        {
            BuildingTimeCurrent += deltaTime;
        }
        else
        {

        }

        // ���콺�� ���� ����. �ٸ� ������ ������ ����.

        // �ϼ��Ǹ� �ϼ��� Material�� �Ѵ�.

        // �Ǽ� �Ϸ�� 
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

}
