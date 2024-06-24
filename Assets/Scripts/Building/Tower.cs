using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Building
{
    protected int powerConsumption; // 타워가 진짜로 소모할 양.
    protected int currentPowerConsumption; // 현재 사용중인 전력 소모량

    protected int attackDamage; // 공격력
    protected float nowTime;
    protected float attackSpeed; // 공격 스피드
    protected float attackRange; // 공격 범위

    protected Monster target; // 타겟 지정된 몬스터
    [SerializeField]protected List<Monster> targetList;
    protected bool onOff; // 꺼졌는지 켜졌는지.

    public void Attack()
    {
        if (!target)
        {
            LockOn();
        }
        else
        {
            // 타워포대를 그 대상을 향해서 옮김.
            if (nowTime <= 0)
            {
                // 공격 대상을 선택. 이미 공격 대상이 정해져있다면 계속 공격.

                // 공격
                // *애니메이션 재생*
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

    protected void OnTriggerEnter(Collider other) // 영역에 들어오면 리스트에 추가
    {
        Debug.Log(other.gameObject.name);
        other.gameObject.TryGetComponent<Monster>(out Monster monster);

        targetList.Add(monster);
        

    }
    protected void OnTriggerExit(Collider other) // 영역에서 나가면 리스트에서 제외
    {
        other.gameObject.TryGetComponent<Monster>(out Monster monster);
        
            targetList.Remove(monster);
        
    }


    public virtual void LockOn()
    {
        target = targetList[0];
    }


    public void TurnOnOff(bool power) //전원을 키고 끌때.
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
