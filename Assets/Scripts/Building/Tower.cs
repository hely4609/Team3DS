using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Building
{
    protected int powerConsumption; // Ÿ���� ��¥�� �Ҹ��� ��.
    protected int currentPowerConsumption; // ���� ������� ���� �Ҹ�

    protected int attackDamage; // ���ݷ�
    protected float nowTime;
    protected float attackSpeed; // ���� ���ǵ�
    protected float attackRange; // ���� ����

    protected Monster target; // Ÿ�� ������ ����
    [SerializeField]protected List<Monster> targetList;
    protected bool onOff; // �������� ��������.

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
                //target.TakeDamage();
                //if (target.hpCurrent <= 0)
                //{
                //    Destroy(target.gameObject);
                //    targetList.Remove(target);
                //    target = null;
                //}
                nowTime = attackSpeed;
            }
            else
            {
                nowTime += Time.deltaTime;
            }
        }
    }

    protected void OnTriggerEnter(Collider other) // ������ ������ ����Ʈ�� �߰�
    {
        Debug.Log(other.gameObject.name);
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
        target = targetList[0];
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
