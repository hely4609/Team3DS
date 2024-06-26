using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Building
{
    protected int powerConsumption; // Ÿ���� ��¥�� �Ҹ��� ��.
    protected int currentPowerConsumption; // ���� ������� ���� �Ҹ�

    protected int attackDamage; // ���ݷ�
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    protected float nowTime;
    protected float attackSpeed; // ���� ���ǵ�
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    protected float attackRange; // ���� ����
    public float AttackRange { get { return attackRange; } set { attackRange = value; AttackRangeSetting(); } }

    protected Monster target; // Ÿ�� ������ ����
    [SerializeField] protected List<Monster> targetList;
    protected bool onOff; // �������� ��������.
    
    protected override void Initialize()
    {
        // ����Ʈ ��.
        type = BuildingEnum.Tower;
        AttackDamage = 1;
        AttackSpeed = 1;
        AttackRange = 2;
        buildingTimeMax = 10;
        size = new Vector2Int(10, 10);
    }

    protected override void MyUpdate(float deltaTime)
    {
        Attack();
    }

    public void Attack()
    {
        if (!target)
        {
            LockOn();
        }
        else
        {
            // Ÿ�����븦 �� ����� ���ؼ� �ű�.
            if (nowTime <= 0)
            {
                // ���� ����� ����. �̹� ���� ����� �������ִٸ� ��� ����.

                // ����
                // *�ִϸ��̼� ���*
                OnHit();

                if (target.HpCurrent <= 0)
                {
                    // �ı��Ҷ�.
                    Destroy(target.gameObject);
                    targetList.Remove(target);
                    target = null;
                }
                nowTime = attackSpeed;
            }
            else
            {
                nowTime -= Time.deltaTime;
            }
        }
    }

    protected virtual void OnHit()
    {
        target.TakeDamage(this, attackDamage);
    }

    protected void OnTriggerEnter(Collider other) // ������ ������ ����Ʈ�� �߰�
    {
        other.gameObject.TryGetComponent<Monster>(out Monster monster);
        targetList.Add(monster);
    }
    protected void OnTriggerExit(Collider other) // �������� ������ ����Ʈ���� ����
    {
        other.gameObject.TryGetComponent<Monster>(out Monster monster);
        targetList.Remove(monster);
    }


    public virtual void LockOn()
    {
        if (targetList.Count > 0)
        {
            target = targetList[0];
            Debug.Log("targeting ready");
        }
    }

    public virtual void AttackRangeSetting()
    {
        TryGetComponent<SphereCollider>(out SphereCollider col);
        col.radius = attackRange;
    }

    public void TurnOnOff(bool power) //������ Ű�� ����.
    {
        if (power != onOff)
        {
            if (power)
            {
                currentPowerConsumption = powerConsumption;
            }
            else
            {
                currentPowerConsumption = 0;
            }
        }
    }
}
