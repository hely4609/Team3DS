using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : InteractableBuilding
{
    protected const float rangeConst = 15f;

    [SerializeField] protected int powerConsumption; // Ÿ���� ��¥�� �Ҹ��� ��.
    protected int currentPowerConsumption; // ���� ������� ���� �Ҹ�

    [SerializeField]protected int attackDamage; // ���ݷ�
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    protected float nowTime;
    [SerializeField]protected float attackSpeed; // ���� ���ǵ�
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    [SerializeField]protected float attackRange; // ���� ����
    public float AttackRange { get { return attackRange; } set { attackRange = value; AttackRangeSetting(); } }

    [SerializeField] protected Monster target = null; // Ÿ�� ������ ����
    [SerializeField] protected List<Monster> targetList = new List<Monster>();
    [SerializeField, Networked] protected bool OnOff { get; set; } // �������� ��������.

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
            // Ÿ�����븦 �� ����� ���ؼ� �ű�.
            gunBarrel.transform.LookAt(target.transform);
            gunBarrel.transform.eulerAngles = new Vector3(0, gunBarrel.transform.eulerAngles.y + gunBarrelRotateCorrection, 0);
            if (nowTime <= 0)
            {
                // ���� ����� ����. �̹� ���� ����� �������ִٸ� ��� ����.

                // ����
                // *�ִϸ��̼� ���*
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

    protected void OnTriggerEnter(Collider other) // ������ ������ ����Ʈ�� �߰�
    {
        other.gameObject.TryGetComponent<Monster>(out Monster monster);
        if (monster != null)
        {
            targetList.Add(monster);
            monster.destroyFunction += MonsterListOut; 
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
            monster.destroyFunction -= MonsterListOut;
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
        }
        else
        {
            Debug.Log("���� ���� �ݶ��̴��� �������� �ʽ��ϴ�.");
        }
    }

    public void TurnOnOff(bool power) //������ Ű�� ���� �Լ�
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
