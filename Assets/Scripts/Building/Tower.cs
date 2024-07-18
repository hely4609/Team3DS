using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
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

    [SerializeField] protected Monster target = null; // Ÿ�� ������ ����
    [SerializeField] protected List<Monster> targetList = new List<Monster>();
    [SerializeField] protected bool onOff ; // �������� ��������.
    
    protected override void Initialize()
    {
        // ����Ʈ ��.
        type = BuildingEnum.Tower;
        isNeedLine = true;
        AttackDamage = 1;
        AttackSpeed = 0.5f;
        AttackRange = 10;
        buildingTimeMax = 10;
        size = new Vector2Int(4, 4);
        TurnOnOff(true);
    }
    public override Interaction InteractionStart(Player player)
    {
        // �ϼ��� ���� �ȵ�.
        if (completePercent < 1)
        {
            return Interaction.Build;
        }
        else
        {
            // ���� ����.
            return Interaction.OnOff;
        }
    }
    protected override void MyUpdate(float deltaTime)
    {
        if(onOff)
        {
            Attack();
        }
        
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

        Debug.Log($"{gameObject.name}");
    }

    protected void OnTriggerEnter(Collider other) // ������ ������ ����Ʈ�� �߰�
    {
        other.gameObject.TryGetComponent<TestMonster>(out TestMonster monster);
        if (monster != null)
        {
            targetList.Add(monster);
            monster.destroyFunction += MonsterListOut; 
        }
    }
    protected void OnTriggerExit(Collider other) // �������� ������ ����Ʈ���� ����
    {
        other.gameObject.TryGetComponent<TestMonster>(out TestMonster monster);
        if (monster != null)
        {
            if (target == monster)
            { target = null; }
            targetList.Remove(monster);
            monster.destroyFunction -= MonsterListOut;
        }
    }

    protected void MonsterListOut(TestMonster monster) // ����¥�� �Լ� ���� ���� : ��������Ʈ�� �ְ� ���� �Ҽ� �ְ��Ϸ���. ���ٽ��� ���� �Ұ���.
    {
        targetList.Remove(monster);
    }



    public virtual void LockOn()
    {
        if (targetList.Count > 0)
        {
            target = targetList[0];
            Debug.Log("targeting ready");
        }
        else
        {
            target = null;
        }
    }

    public virtual void AttackRangeSetting()
    {
        if(gameObject.TryGetComponent<SphereCollider>(out SphereCollider col))
        {
            col.radius = attackRange;
            col.center = new Vector3(0, col.radius * 0.5f, 0);
        }
        else
        {
            Debug.Log("���� ���� �ݶ��̴��� �������� �ʽ��ϴ�.");
        }
    }

    public void TurnOnOff(bool power) //������ Ű�� ���� �Լ�
    {
        if (power != onOff)
        {
            onOff = power;
            if (onOff)
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
