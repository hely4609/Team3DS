using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Building
{
    protected int powerConsumption; // Ÿ���� ��¥�� �Ҹ��� ��.
    protected int currentPowerConsumption; // ���� ������� ���� �Ҹ�

    [SerializeField]protected int attackDamage; // ���ݷ�
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    protected float nowTime;
    [SerializeField]protected float attackSpeed; // ���� ���ǵ�
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    protected float attackRange; // ���� ����
    public float AttackRange { get { return attackRange; } set { attackRange = value; AttackRangeSetting(); } }

    [SerializeField]protected Monster target; // Ÿ�� ������ ����
    [SerializeField] protected List<Monster> targetList;
    protected bool onOff; // �������� ��������.
    
    protected override void Initialize()
    {
        // ����Ʈ ��.
        type = BuildingEnum.Tower;
        AttackDamage = 1;
        AttackSpeed = 0.5f;
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
                    targetList.Remove(target);
                    Destroy(target.gameObject);
                    target = null;
                    Debug.Log("dd");
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
        if (monster != null)
        {
            targetList.Add(monster);
        }
    }
    protected void OnTriggerExit(Collider other) // �������� ������ ����Ʈ���� ����
    {
        other.gameObject.TryGetComponent<Monster>(out Monster monster);
        if (monster != null)
        {
            if (target == monster)
            { target = null; }
            targetList.Remove(monster);
        }
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
