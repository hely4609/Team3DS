using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : InteractableBuilding
{
    protected const float rangeConst = 15f;

    [SerializeField] protected int powerConsumption; // 타워가 진짜로 소모할 양.
    protected int currentPowerConsumption; // 현재 사용중인 전력 소모량

    [SerializeField]protected int attackDamage; // 공격력
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    protected float nowTime;
    [SerializeField]protected float attackSpeed; // 공격 스피드
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    [SerializeField]protected float attackRange; // 공격 범위
    public float AttackRange { get { return attackRange; } set { attackRange = value; AttackRangeSetting(); } }

    [SerializeField] protected Monster target = null; // 타겟 지정된 몬스터
    [SerializeField] protected List<Monster> targetList = new List<Monster>();
    [SerializeField, Networked] protected bool OnOff { get; set; } // 꺼졌는지 켜졌는지.

    [SerializeField] protected Animator animator;
    [SerializeField] protected GameObject gunBarrel;
    [SerializeField] protected float gunBarrelRotateCorrection;

    public override void Spawned()
    {
        base.Spawned();
        if (CompletePercent >= 1)
        {
            marker_on.SetActive(OnOff);
            marker_off.SetActive(!OnOff);
            foreach (MeshRenderer r in meshes)
            {
                r.material.SetFloat("_OnOff", OnOff ? 1f : 0f);
            }
        }
    }


    public override Interaction InteractionStart(Player player)
    {
        // 완성이 아직 안됨.
        if (CompletePercent < 1)
        {
            return Interaction.Build;
        }

        else if (IsRoped)
        {
            // 전원 끄기. 반대 상태로 토글합니다.
            TurnOnOff(!OnOff);
            return Interaction.OnOff;
        }
        else if (player.ropeBuilding == null)
        {
            Vector2 playerTransformVector2 = new Vector2((int)(player.transform.position.x), (int)(player.transform.position.z));
            if (!IsSettingRope && player.ropeBuilding == null)
            {
                OnRopeSet(playerTransformVector2, 0);
                player.ropeBuilding = this;
                return Interaction.takeRope;
            }
            else return Interaction.None;
        }
        else
        {
            AttachRope(player.ropeBuilding, 0);
            player.ropeBuilding = null;
            return Interaction.None;
        }
    }
    protected override void MyUpdate(float deltaTime)
    {
        if(OnOff)
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
            gunBarrel.transform.LookAt(target.transform);
            gunBarrel.transform.eulerAngles = new Vector3(0, gunBarrel.transform.eulerAngles.y + gunBarrelRotateCorrection, 0);
            if (nowTime <= 0)
            {
                // 공격 대상을 선택. 이미 공격 대상이 정해져있다면 계속 공격.

                // 공격
                // *애니메이션 재생*
                animator.SetTrigger("Attack");
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
        if (HasStateAuthority)
        {
            target.TakeDamage(this, attackDamage);
        }

        if (!target.IsReady)
        {
            MonsterListOut(target);
            target = null;
        }

        Debug.Log($"{gameObject.name}");
    }

    protected void OnTriggerEnter(Collider other) // 영역에 들어오면 리스트에 추가
    {
        other.gameObject.TryGetComponent<Monster>(out Monster monster);
        if (monster != null)
        {
            targetList.Add(monster);
            monster.destroyFunction += MonsterListOut; 
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
            monster.destroyFunction -= MonsterListOut;
        }
    }

    protected void MonsterListOut(Monster monster) // 한줄짜리 함수 만든 이유 : 델리게이트에 넣고 빼기 할수 있게하려고. 람다식은 빼기 불가능.
    {
        // 몬스터를 타겟 리스트에서 제거한다.
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

    // 공격범위 세팅
    public virtual void AttackRangeSetting()
    {
        if(gameObject.TryGetComponent<SphereCollider>(out SphereCollider col))
        {
            Debug.Log(transform.localScale.x);
            col.radius = attackRange * rangeConst / transform.localScale.x;
            //col.center = new Vector3(0, col.radius * 0.5f, 0);
        }
        else
        {
            Debug.Log("공격 범위 콜라이더가 존재하지 않습니다.");
        }
    }

    public void TurnOnOff(bool power) //전원을 키고 끄는 함수
    {
        SoundManager.Play(ResourceEnum.SFX._switch, transform.position);
        if (HasStateAuthority && power != OnOff)
        {
            OnOff = power;
            if (OnOff)
            {
                currentPowerConsumption = powerConsumption;
            }
            else
            {
                currentPowerConsumption = 0;
            }
        }
    }

    public override void Render()
    {
        foreach(var chage in _changeDetector.DetectChanges(this))
        {
            switch(chage)
            {
                case nameof(isBuildable):
                    VisualizeBuildable();
                    break;


                case nameof(IsFixed):
                    foreach (Collider col in cols)
                    {
                        col.enabled = true;
                    }
                    break;

                case nameof(BuildingTimeCurrent):
                    {
                        foreach (MeshRenderer r in meshes)
                        {
                            r.material.SetFloat("_CompletePercent", CompletePercent);
                        }

                        if (CompletePercent >= 1)
                        {
                            foreach (MeshRenderer r in meshes)
                            {
                                r.material = completeMat;
                            }
                            TurnOnOff(false);
                            marker_designed.SetActive(false);
                            marker_on.SetActive(true);
                        }
                    }
                    break;
                case nameof(OnOff):
                    marker_on.SetActive(OnOff);
                    marker_off.SetActive(!OnOff);
                    foreach (MeshRenderer r in meshes)
                    {
                        r.material.SetFloat("_OnOff", OnOff? 1f : 0f);
                    }

                    GameManager.Instance.BuildingManager.supply.ChangePowerConsumption(OnOff ? -powerConsumption : powerConsumption); 
                    break;
            }
        }
    }
    public override void AttachRope(InteractableBuilding building, int number)
    {
        if (building is Pylon)
        //if (building.GetType().IsSubclassOf(typeof(Tower)))
        {
            Vector2 thisVector2 = new Vector2((int)(transform.position.x), (int)(transform.position.z));

            building.OnRopeSet(thisVector2, number);
            IsSettingRope = false;
            IsRoped = true;
        }
    }
}
