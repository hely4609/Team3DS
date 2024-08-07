using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterEnum
{ First }

public delegate void MonsterDestroyFunction(Monster monster);

public class Monster : Character
{
    protected int oreAmount;
    protected List<Vector2> roadsVector2; // �� ���� �迭
    [SerializeField]protected int roadDestination; // ���� ���� ���ϰ� �ִ°�.
    protected MonsterEnum monsterType;
    public override void Attack(Character target) { }

    bool isRelease = false;
    bool isReady = false;

    public MonsterDestroyFunction destroyFunction;
    protected EnergyBarrierGenerator generator;
    protected override void MyStart()
    {
        Debug.Log("monster Mystart");
        hpMax = 5;
        hpCurrent = 5;
        roadsVector2 = GameManager.Instance.BuildingManager.roadData;
        roadDestination = roadsVector2.Count-1;
        monsterType = MonsterEnum.First;
        isReady= true;

        generator = GameManager.Instance.BuildingManager.generator;
    }

    public override int TakeDamage(Tower attacker, int damage)
    {
            hpCurrent -= damage;
            if (hpCurrent <= 0)
            {
                destroyFunction.Invoke(this);
                Destroy(gameObject);
            }
            Debug.Log($"{HpCurrent} / {gameObject.name}");
        
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
}
