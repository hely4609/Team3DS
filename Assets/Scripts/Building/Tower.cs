using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : InteractableBuilding
{
    protected const float rangeConst = 15f;

    [SerializeField, Networked] protected int powerConsumption { get; set; } // Ÿ���� ��¥�� �Ҹ��� ��.
    protected int currentPowerConsumption; // ���� ������� ���� �Ҹ�

    [SerializeField, Networked] protected int attackDamage { get; set; } // ���ݷ�
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    protected float nowTime;
    [SerializeField, Networked] protected float attackSpeed { get; set; } // ���� ���ǵ�
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    [SerializeField, Networked] protected float attackRange { get; set; } // ���� ����
    public float AttackRange { get { return attackRange; } set { attackRange = value; AttackRangeSetting(); } }

    [SerializeField] protected Monster target = null; // Ÿ�� ������ ����
    [SerializeField] protected List<Monster> targetList = new List<Monster>();
    [SerializeField, Networked] protected bool OnOff { get; set; } // �������� ��������.

    [SerializeField] protected Animator attackAnimator;
    [SerializeField] protected GameObject gunBarrel;
    [SerializeField] protected float gunBarrelRotateCorrection;
    public Pylon attachedPylon;

    [SerializeField] protected GameObject attackRangeMarker;

    public override void Spawned()
    {
        base.Spawned();
        attackAnimator.SetFloat("AttackSpeed", 1 / attackSpeed);
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


    public override Interaction InteractionStart(Player player)
    {
        // �ϼ��� ���� �ȵ�.
        if (CompletePercent < 1)
        {
            return Interaction.Build;
        }

        else if (IsRoped)
        {
            // ���� ����. �ݴ� ���·� ����մϴ�.
            TurnOnOff(!OnOff);
            return Interaction.OnOff;
        }
        else if (player.ropeBuilding == null)
        {
            Vector3 playerTransformVector3 = new Vector3((int)(player.transform.position.x), (int)(player.transform.position.y), (int)(player.transform.position.z));
            if (!IsSettingRope && player.ropeBuilding == null)
            {
                OnRopeSet(playerTransformVector3, 0);
                player.ropeBuilding = this;
                return Interaction.takeRope;
            }
            else return Interaction.None;
        }
        else
        {
            AttachRope(player, player.PossesionController.MyNumber);
            return Interaction.None;
        }
    }
    protected override void MyUpdate(float deltaTime)
    {
        if(HasStateAuthority && OnOff)
        {
            Attack();
        }
        
    }

    public void Attack()
    {
        if (!target)
        {
            attackAnimator.SetBool("Attack", false);
            LockOn();
            
        }
        else
        {
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

        if (HasStateAuthority)
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
