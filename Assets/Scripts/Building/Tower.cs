using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Building
{
    protected int powerConsumption; // 타워가 진짜로 소모할 양.
    protected int currentPowerConsumption; // 현재 사용중인 전력 소모량

    [SerializeField]protected int attackDamage; // 공격력
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    protected float nowTime;
    [SerializeField]protected float attackSpeed; // 공격 스피드
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    protected float attackRange; // 공격 범위
    public float AttackRange { get { return attackRange; } set { attackRange = value; AttackRangeSetting(); } }

    [SerializeField]protected Monster target; // 타겟 지정된 몬스터
    [SerializeField] protected List<Monster> targetList;
    protected bool onOff; // 꺼졌는지 켜졌는지.
    
    protected override void Initialize()
    {
        // 디폴트 값.
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
            // 타워포대를 그 대상을 향해서 옮김.
            if (nowTime <= 0)
            {
                // 공격 대상을 선택. 이미 공격 대상이 정해져있다면 계속 공격.

                // 공격
                // *애니메이션 재생*
                OnHit();

                if (target.HpCurrent <= 0)
                {
                    // 파괴할때.
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

    protected void OnTriggerEnter(Collider other) // 영역에 들어오면 리스트에 추가
    {
        other.gameObject.TryGetComponent<Monster>(out Monster monster);
        if (monster != null)
        {
            targetList.Add(monster);
        }
    }
    protected void OnTriggerExit(Collider other) // 영역에서 나가면 리스트에서 제외
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
