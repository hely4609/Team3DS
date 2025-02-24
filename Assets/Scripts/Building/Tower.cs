using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : InteractableBuilding
{
    protected const float rangeConst = 15f;

    [SerializeField, Networked] public int powerConsumption { get; set; } // 타워가 진짜로 소모할 양.
    protected int currentPowerConsumption; // 현재 사용중인 전력 소모량

    [SerializeField, Networked] protected int attackDamage { get; set; } // 공격력
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    [SerializeField] int attackUpgradeIncrease;
    public int AttackUpgradeIncrease => attackUpgradeIncrease;
    protected float nowTime;
    [SerializeField, Networked] protected float attackSpeed { get; set; } // 공격 스피드
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    [SerializeField, Networked] protected float attackRange { get; set; } // 공격 범위
    public float AttackRange { get { return attackRange; } set { attackRange = value; AttackRangeSetting(); } }

    [SerializeField] protected Monster target = null; // 타겟 지정된 몬스터
    [SerializeField] protected List<Monster> targetList = new List<Monster>();
    [SerializeField, Networked] public bool OnOff { get; set; } // 꺼졌는지 켜졌는지.

    [SerializeField] protected Animator attackAnimator;
    [SerializeField] protected GameObject gunBarrel;
    [SerializeField] protected float gunBarrelRotateCorrection;
    public Pylon attachedPylon;

    [SerializeField] protected GameObject attackRangeMarker;
    [SerializeField, Networked] protected int Level { get; set; }
    [SerializeField, Networked] public int UpgradeRequire { get; set; }
    [Networked] public int TotalUpgradeCost { get; set; }

    

    public override void Spawned()
    {
        base.Spawned();
        attackAnimator.SetFloat("AttackSpeed", GameManager.Instance.BuildingManager.generator.GameSpeed / attackSpeed);
        if (CompletePercent >= 1)
        {
            marker_on.SetActive(OnOff);
            marker_off.SetActive(!OnOff);
            foreach (MeshRenderer r in meshes)
            {
                r.material.SetFloat("_OnOff", OnOff ? 1f : 0f);
            }
            buildingSignCanvas.SetActive(!IsRoped);
        }
    }

    public override List<Interaction> GetInteractions(Player player)
    {
        List<Interaction> currentAbleInteractions = new List<Interaction>();

        if (CompletePercent < 1)
        {
            currentAbleInteractions.Add(Interaction.Build);
            currentAbleInteractions.Add(Interaction.Demolish);
        }
        else if (IsRoped)
        {
            currentAbleInteractions.Add(Interaction.OnOff);
            currentAbleInteractions.Add(Interaction.Demolish);
            currentAbleInteractions.Add(Interaction.Upgrade);
        }
        else if (player.ropeBuilding == null)
        {
            if (!IsSettingRope && player.ropeBuilding == null)
            {
                currentAbleInteractions.Add(Interaction.takeRope);
                currentAbleInteractions.Add(Interaction.Demolish);
            }

        }
        else if(player.ropeBuilding != this) 
        {
            currentAbleInteractions.Add(Interaction.AttachRope);
        }
        return currentAbleInteractions;
    }
    public override Interaction InteractionStart(Player player, Interaction interactionType)
    {
        
        switch (interactionType)
        {
            default:
                break;
            case Interaction.Build:
                Debug.Log("건설중");
                break;
            case Interaction.None:
                Debug.Log("None");
                break;
            case Interaction.takeRope:
                Debug.Log("전선들기");
                Vector3 playerTransformVector3 = new Vector3((int)(player.transform.position.x), (int)(player.transform.position.y), (int)(player.transform.position.z));
                if (!IsSettingRope && player.ropeBuilding == null)
                {
                    OnRopeSet(playerTransformVector3, 0);
                    player.ropeBuilding = this;
                }
                break;
            case Interaction.Demolish:
                Debug.Log("해체");
                int returnCost;
                if (CompletePercent < 1) returnCost = cost + TotalUpgradeCost;
                else returnCost = (int)((cost + TotalUpgradeCost) * 0.7f + 0.5f);

                GameManager.Instance.BuildingManager.supply.TotalOreAmount += returnCost;

                Runner.Despawn(GetComponent<NetworkObject>());

                break;
            case Interaction.OnOff:
                Debug.Log("켜기/끄기");
                TurnOnOff(!OnOff);
                //IsChangeInfo = !IsChangeInfo;
                break;
            case Interaction.AttachRope:
                AttachRope(player, player.PossesionController.MyNumber);
                //IsChangeInfo = !IsChangeInfo;
                break;
            case Interaction.Upgrade:
                Debug.Log("업그레이드");
                Upgrade();
                //IsChangeInfo = !IsChangeInfo;
                break;

        }

        
        return interactionType;

        //// 완성이 아직 안됨.
        //if (CompletePercent < 1)
        //{
        //    return Interaction.Build;
        //}

        //else if (IsRoped)
        //{
        //    // 전원 끄기. 반대 상태로 토글합니다.
        //    TurnOnOff(!OnOff);
        //    return Interaction.OnOff;
        //}
        //else if (player.ropeBuilding == null)
        //{
        //    Vector3 playerTransformVector3 = new Vector3((int)(player.transform.position.x), (int)(player.transform.position.y), (int)(player.transform.position.z));
        //    if (!IsSettingRope && player.ropeBuilding == null)
        //    {
        //        OnRopeSet(playerTransformVector3, 0);
        //        player.ropeBuilding = this;
        //        return Interaction.takeRope;
        //    }
        //    else return Interaction.None;
        //}
        //else
        //{
        //    AttachRope(player, player.PossesionController.MyNumber);
        //    return Interaction.None;
        //}
    }

    public override bool InteractionEnd(Player player, Interaction interactionType)
    {
        switch (interactionType)
        {
            case Interaction.Build:
            case Interaction.Upgrade:
            case Interaction.OnOff:
            case Interaction.AttachRope:
                if(HasStateAuthority) IsChangeInfo = !IsChangeInfo;
                break;
        }

        return true;
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (!GameManager.IsGameStart) return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (HasStateAuthority)
        {
            GameManager.Instance.BuildingManager.RemoveBuilding(this);

            foreach(var player in players)
            {
                var playerCS = player.GetComponent<Player>();
                if (playerCS.ropeBuilding == this)
                {
                    playerCS.CanSetRope = true;
                }
            }
        }

        foreach(var player in players)
        {
            var playerCS = player.GetComponent<Player>();
            playerCS.RenewalInteractionUI(this, false);
        }

        foreach (var rope in ropeStruct.ropeObjects)
        {
            Runner.Despawn(rope);
        }

    }

    protected override void MyUpdate(float deltaTime)
    {
        if(HasStateAuthority && OnOff)
        {
            attackAnimator.SetFloat("AttackSpeed", GameManager.Instance.BuildingManager.generator.GameSpeed / attackSpeed);
            Attack();
        }
        
    }

    public virtual void Attack()
    {
        // 판단기준
        // 타겟이있는가
        // 타겟이있다면 살아있는가(isReady)

        if (!target)
        {
            //attackAnimator.SetBool("Attack", false);
            LockOn();
            
        }
        else
        {
            attackAnimator.SetBool("Attack", false);

            // 타워포대를 그 대상을 향해서 옮김.
            gunBarrel.transform.LookAt(target.transform);
            gunBarrel.transform.eulerAngles = new Vector3(0, gunBarrel.transform.eulerAngles.y + gunBarrelRotateCorrection, 0);
            if (nowTime <= 0)
            {
                // 공격 대상을 선택. 이미 공격 대상이 정해져있다면 계속 공격.

                // 공격
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
        if (!target.isReady)
        {
            MonsterListOut(target);
            target = null;
            Attack();
            return;
        }
        else
        {
            // *애니메이션 재생*
            attackAnimator.SetBool("Attack", true);
            target.TakeDamage(this, attackDamage);
        }

        Debug.Log($"{gameObject.name}");
    }


    protected void OnTriggerEnter(Collider other) // 영역에 들어오면 리스트에 추가
    {
        if (HasStateAuthority)
        {
            other.gameObject.TryGetComponent<Monster>(out Monster monster);
            if (monster != null)
            {
                targetList.Add(monster);
                monster.destroyFunction += MonsterListOut;
            }
        }
      
    }
    protected void OnTriggerExit(Collider other) // 영역에서 나가면 리스트에서 제외
    {
        if (HasStateAuthority)
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

    }

    protected void MonsterListOut(Monster monster) // 한줄짜리 함수 만든 이유 : 델리게이트에 넣고 빼기 할수 있게하려고. 람다식은 빼기 불가능.
    {
        // 몬스터를 타겟 리스트에서 제거한다.
        targetList.Remove(monster);
    }

    public virtual void Upgrade()
    {
        if (!HasStateAuthority) return;
        if (UpgradeRequire > GameManager.Instance.BuildingManager.supply.TotalOreAmount)
        {
            
            if (!GameManager.Instance.NetworkManager.LocalController.ControlledPlayer.AlreadyAlert && HasInputAuthority)
            {
                GameManager.ManagerUpdates += GameManager.Instance.NetworkManager.LocalController.ControlledPlayer.NotEnoughOreAlert;
            }
            Debug.Log("업그레이드에 필요한 광물이 부족합니다.");
            return;
        }
        attackDamage += attackUpgradeIncrease;
        GameManager.Instance.BuildingManager.supply.TotalOreAmount -= UpgradeRequire;
        TotalUpgradeCost += UpgradeRequire;
        UpgradeRequire = Mathf.CeilToInt(UpgradeRequire * 1.1f);
        Level += 1;
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
            attackRangeMarker.transform.localScale = new Vector3(col.radius * 2, col.radius * 2, col.radius * 2);
        }
        else
        {
            Debug.Log("공격 범위 콜라이더가 존재하지 않습니다.");
        }
    }

    public void TurnOnOff(bool power) //전원을 키고 끄는 함수
    {
        if (HasStateAuthority && power != OnOff && attachedPylon.OnOff)
        {
            if (power)
            {
                if ( GameManager.Instance.BuildingManager.supply.PowerCurrent >= powerConsumption)
                {
                    currentPowerConsumption = powerConsumption;
                }
                else
                {
                    return;
                }
            }
            else 
            {
                currentPowerConsumption = 0;
                attackAnimator.SetBool("Attack", false);
            }

            SoundManager.Play(ResourceEnum.SFX._switch, transform.position);
            OnOff = power;
        }
    }

    public override void Render()
    {
        foreach(var chage in _changeDetector.DetectChanges(this))
        {
            switch(chage)
            {
                case nameof(IsChangeInfo):
                    Player local = GameManager.Instance.NetworkManager.LocalController.ControlledPlayer;
                    local.RenewalInteractionUI(this);
                    break;

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
                            marker_off.SetActive(true);
                            buildingSignCanvas.transform.localPosition = new Vector3(0, heightMax * 0.5f / transform.localScale.y, 0);
                            buildingSignCanvas.transform.localScale /= transform.localScale.x;
                            buildingSignCanvas.GetComponent<BuildingSignCanvas>().SetRadius(size.x);
                            buildingSignCanvas.SetActive(!IsRoped);
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
                case nameof(IsRoped):
                    buildingSignCanvas.SetActive(!IsRoped);
                    break;
                case nameof(attackRange):
                    AttackRangeSetting();
                    break;
            }
        }
    }
    public override void AttachRope(Player player, int number)
    {
        if (player.PossesionController.myAuthority == Runner.LocalPlayer)
        {
            player.ropeMaxDistanceSignUI.SetActive(false);
        }
        InteractableBuilding building = player.ropeBuilding;
        if (building is Pylon)
        //if (building.GetType().IsSubclassOf(typeof(Tower)))
        {
            Pylon py = building as Pylon;
            Vector3 thisVector3 = new Vector3((int)(transform.position.x),(int)(transform.position.y) ,(int)(transform.position.z));

            py.OnRopeSet(thisVector3, number);
            IsSettingRope = false;
            
            IsRoped = true;
            buildingSignCanvas.SetActive(false);

            foreach (var item in py.MultiTabList[number].ropeObjects)
            {
                ropeStruct.ropeObjects.Add(item);    
            }

            py.MultiTabList[number].ropeObjects.Clear();
            py.MultiTabList[number].ropePositions.Clear();
            py.ResetRope(player, player.PossesionController.MyNumber);

            player.CanSetRope = true;
            player.ropeBuilding = null;
            attachedPylon = py;
            SoundManager.Play(ResourceEnum.SFX.plug_in, transform.position);
        }
    }
}
