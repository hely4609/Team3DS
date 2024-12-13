using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterEnum
{ First }

public delegate void MonsterDestroyFunction(Monster monster);

public class Monster : Character
{
    [SerializeField] Canvas hpCanvas;
    [SerializeField] Image hpFillImage;
    [SerializeField] TextMeshProUGUI hpText;


    [SerializeField] protected int oreAmount;
    protected List<Vector2> roadsVector2; // �� ���� �迭
    [SerializeField]protected int roadDestination; // ���� ���� ���ϰ� �ִ°�.
    protected MonsterEnum monsterType;


    [SerializeField] protected int hpMax;
    [SerializeField] protected int hpCurrent;
    [Networked] public int HpCurrent { get; set; }
    [SerializeField] protected int attackDamage = 1;
    protected float attackSpeed;

    bool isRelease = false;
    [Networked]public bool isReady { get; set; } = false;
    //public bool IsReady { get { return isReady; } }

    public MonsterDestroyFunction destroyFunction;
    protected EnergyBarrierGenerator generator;

    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }


    protected override void MyStart()
    {
        HpCurrent = hpMax;
        roadsVector2 = GameManager.Instance.BuildingManager.roadData;
        roadDestination = GameManager.Instance.WaveManager.SpawnLoc;
        monsterType = MonsterEnum.First;
        isReady= true;

        generator = GameManager.Instance.BuildingManager.generator;
    }

    public override int TakeDamage(Tower attacker, int damage)
    {
        HpCurrent -= damage;
        
        
        return 0;
    }
        
    // ���Ͱ� �̵��ϴ� ���.
    // ��� ���� �����Ѵ�.
    // ������ ��ġ�� �� �� ���� ����ؼ� �̵��Ѵ�.
    // ���Ͱ� �ش� ��ġ�� �ִٸ�, ���� ��ġ�� �����Ѵ�.
    // �ش� ��ġ�� ��Ȯ�� ������ ���ɼ��� �ſ� �����Ƿ�, ��ġ���� ���������� ���� �ξ� �ش� �αٿ� �����ϸ� �����Ѱɷ� �Ѵ�.

    
    public override void FixedUpdateNetwork()
    {
        if (GameManager.IsGameStart)
        {
            if (isReady)
            {
                MonsterMove(NextDestination(), Runner.DeltaTime);
                if (IsDestinationArrive(NextDestination()))
                {
                    roadDestination--;
                    

                    if (roadDestination <= 0)
                    {
                        roadDestination = 0;
                    }
                }
                if (!generator.OnOff)
                {
                    isRelease = true;
                }

                hpCanvas.transform.LookAt(Camera.main.transform.position);

            }
        }
    }
    protected Vector3 NextDestination()
    {
        Vector3 destVector3;
        if (!isRelease)
        { 
            Vector2 dest = roadsVector2[roadDestination];

            destVector3 = new Vector3(dest.x, transform.position.y, dest.y);


        }
        else
        {
            destVector3 = Vector3.zero;
        }
        return destVector3;
    }

    protected virtual void MonsterMove(Vector3 destination, float deltaTime)
    {
        // �ش�������� ����.
        Vector3 dir = (destination - transform.position).normalized;
        transform.position += dir * deltaTime * MoveSpeed;
        transform.LookAt(destination);
    }

    public void ReleaseMonster()
    {
        isRelease= true;
    }

    public bool IsDestinationArrive(Vector3 destination)
    {
        // ���� �Ÿ���, ��ǥ����+�������� �ȿ� �ִ��� Ȯ��. 
        // ���� �Ÿ���, ��ǥ����-�������� �ȿ� �ִ��� Ȯ��.
        // 
        float errorRange = 1;
        Vector3 oneVector = new Vector3(1, 0, 1);
        Vector3 upDestination = destination + oneVector*errorRange;
        Vector3 downDestination = destination - oneVector*errorRange;

        if (transform.position.x < upDestination.x && transform.position.z < upDestination.z)
        {
            if(transform.position.x > downDestination.x && transform.position.z > downDestination.z) 
            { 
                return true;
            }
        }
        return false;
        
    }

    public void Dead()
    {
        if (HasStateAuthority)
        {
            GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Ore), transform.position);
        }
        
        Despawn();
    }

    public void Despawn()
    {
        Runner.Despawn(GetComponent<NetworkObject>());
    }

    public void OnDestroyFunction()
    {
        destroyFunction?.Invoke(this);
    }

    public void Attack(EnergyBarrierGenerator target) 
    {
        if (!isReady) return;
        isReady = false;

        AnimTrigger?.Invoke("AttackTrigger");
        AnimBool?.Invoke("isMove", false);

        target.TakeDamage(attackDamage);
        GameManager.Instance.WaveManager.MonsterCount--;
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(HpCurrent):
                    hpText.text = $"{HpCurrent} / {hpMax}";
                    hpFillImage.fillAmount = HpCurrent / (float)hpMax;
                    if (HpCurrent <= 0)
                    {
                        isReady = false;

                        if (HasStateAuthority)
                        {
                            GameManager.Instance.BuildingManager.generator.KillCount++;
                            GameManager.Instance.WaveManager.MonsterCount--;
                        }
                        GetComponent<Collider>().enabled = false;
                        GetComponent<Rigidbody>().isKinematic = true;

                        AnimTrigger?.Invoke("DieTrigger");
                        AnimBool?.Invoke("isMove", false);
                        //  destroyFunction.Invoke(this);
                        //  Runner.Despawn(GetComponent<NetworkObject>());
                    }
                    break;
            }
        }
    }
}
