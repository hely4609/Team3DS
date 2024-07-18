using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
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

    [SerializeField] protected Monster target = null; // 타겟 지정된 몬스터
    [SerializeField] protected List<Monster> targetList = new List<Monster>();
    [SerializeField] protected bool onOff ; // 꺼졌는지 켜졌는지.
    
    protected override void Initialize()
    {
        // 디폴트 값.
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
        // 완성이 아직 안됨.
        if (completePercent < 1)
        {
            return Interaction.Build;
        }
        else
        {
            // 전원 끄기.
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
            // 타워포대를 그 대상을 향해서 옮김.
            if (nowTime <= 0)
            {
                // 공격 대상을 선택. 이미 공격 대상이 정해져있다면 계속 공격.

                // 공격
                // *애니메이션 재생*
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

    protected void OnTriggerEnter(Collider other) // 영역에 들어오면 리스트에 추가
    {
        other.gameObject.TryGetComponent<TestMonster>(out TestMonster monster);
        if (monster != null)
        {
            targetList.Add(monster);
            monster.destroyFunction += MonsterListOut; 
        }
    }
    protected void OnTriggerExit(Collider other) // 영역에서 나가면 리스트에서 제외
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

    protected void MonsterListOut(TestMonster monster) // 한줄짜리 함수 만든 이유 : 델리게이트에 넣고 빼기 할수 있게하려고. 람다식은 빼기 불가능.
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
            Debug.Log("공격 범위 콜라이더가 존재하지 않습니다.");
        }
    }

    public void TurnOnOff(bool power) //전원을 키고 끄는 함수
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
