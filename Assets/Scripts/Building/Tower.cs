using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : InteractableBuilding
{
    protected const float rangeConst = 15f;

    [SerializeField, Networked] public int powerConsumption { get; set; } // Ÿ���� ��¥�� �Ҹ��� ��.
    protected int currentPowerConsumption; // ���� ������� ���� �Ҹ�

    [SerializeField, Networked] protected int attackDamage { get; set; } // ���ݷ�
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    [SerializeField] int attackUpgradeIncrease;
    public int AttackUpgradeIncrease => attackUpgradeIncrease;
    protected float nowTime;
    [SerializeField, Networked] protected float attackSpeed { get; set; } // ���� ���ǵ�
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    [SerializeField, Networked] protected float attackRange { get; set; } // ���� ����
    public float AttackRange { get { return attackRange; } set { attackRange = value; AttackRangeSetting(); } }

    [SerializeField] protected Monster target = null; // Ÿ�� ������ ����
    [SerializeField] protected List<Monster> targetList = new List<Monster>();
    [SerializeField, Networked] public bool OnOff { get; set; } // �������� ��������.

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
                Debug.Log("�Ǽ���");
                break;
            case Interaction.None:
                Debug.Log("None");
                break;
            case Interaction.takeRope:
                Debug.Log("�������");
                Vector3 playerTransformVector3 = new Vector3((int)(player.transform.position.x), (int)(player.transform.position.y), (int)(player.transform.position.z));
                if (!IsSettingRope && player.ropeBuilding == null)
                {
                    OnRopeSet(playerTransformVector3, 0);
                    player.ropeBuilding = this;
                }
                break;
            case Interaction.Demolish:
                Debug.Log("��ü");
                int returnCost;
                if (CompletePercent < 1) returnCost = cost + TotalUpgradeCost;
                else returnCost = (int)((cost + TotalUpgradeCost) * 0.7f + 0.5f);

                GameManager.Instance.BuildingManager.supply.TotalOreAmount += returnCost;

                Runner.Despawn(GetComponent<NetworkObject>());

                break;
            case Interaction.OnOff:
                Debug.Log("�ѱ�/����");
                TurnOnOff(!OnOff);
                //IsChangeInfo = !IsChangeInfo;
                break;
            case Interaction.AttachRope:
                AttachRope(player, player.PossesionController.MyNumber);
                //IsChangeInfo = !IsChangeInfo;
                break;
            case Interaction.Upgrade:
                Debug.Log("���׷��̵�");
                Upgrade();
                //IsChangeInfo = !IsChangeInfo;
                break;

        }

        
        return interactionType;

        //// �ϼ��� ���� �ȵ�.
        //if (CompletePercent < 1)
        //{
        //    return Interaction.Build;
        //}

        //else if (IsRoped)
        //{
        //    // ���� ����. �ݴ� ���·� ����մϴ�.
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
        // �Ǵܱ���
        // Ÿ�����ִ°�
        // Ÿ�����ִٸ� ����ִ°�(isReady)

        if (!target)
        {
            //attackAnimator.SetBool("Attack", false);
            LockOn();
            
        }
        else
        {
            attackAnimator.SetBool("Attack", false);

            // Ÿ�����븦 �� ����� ���ؼ� �ű�.
            gunBarrel.transform.LookAt(target.transform);
            gunBarrel.transform.eulerAngles = new Vector3(0, gunBarrel.transform.eulerAngles.y + gunBarrelRotateCorrection, 0);
            if (nowTime <= 0)
            {
                // ���� ����� ����. �̹� ���� ����� �������ִٸ� ��� ����.

                // ����
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
            // *�ִϸ��̼� ���*
            attackAnimator.SetBool("Attack", true);
            target.TakeDamage(this, attackDamage);
        }

        Debug.Log($"{gameObject.name}");
    }


    protected void OnTriggerEnter(Collider other) // ������ ������ ����Ʈ�� �߰�
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
    protected void OnTriggerExit(Collider other) // �������� ������ ����Ʈ���� ����
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

    protected void MonsterListOut(Monster monster) // ����¥�� �Լ� ���� ���� : ��������Ʈ�� �ְ� ���� �Ҽ� �ְ��Ϸ���. ���ٽ��� ���� �Ұ���.
    {
        // ���͸� Ÿ�� ����Ʈ���� �����Ѵ�.
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
            Debug.Log("���׷��̵忡 �ʿ��� ������ �����մϴ�.");
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

    // ���ݹ��� ����
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
            Debug.Log("���� ���� �ݶ��̴��� �������� �ʽ��ϴ�.");
        }
    }

    public void TurnOnOff(bool power) //������ Ű�� ���� �Լ�
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
