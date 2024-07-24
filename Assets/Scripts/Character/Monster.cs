using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterEnum
{ }

public delegate void MonsterDestroyFunction(Monster monster);

public class Monster : Character
{
    protected int oreAmount;
    protected List<Vector2> roadsVector2; // �� ���� �迭
    protected int roadDestination; // ���� ���� ���ϰ� �ִ°�.
    public override void Attack(Character target) { }

    bool isRelease = false;

    public MonsterDestroyFunction destroyFunction;
    protected override void MyStart()
    {
        hpMax = 5;
        hpCurrent = 5;
        roadsVector2 = GameManager.Instance.BuildingManager.roadData;
        roadDestination = 0;
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

    protected Vector3 NextDestination()
    {

        Vector2 dest = roadsVector2[roadDestination];
        return new Vector3(dest.x, transform.position.y, dest.y);
    }

    protected virtual void MonsterMove(Vector3 destination)
    {
        
    }

    public void ReleaseMonster()
    {
        isRelease= true;
    }
}
