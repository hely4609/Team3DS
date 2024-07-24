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
    protected List<Vector2> roadsVector2; // 길 정보 배열
    protected int roadDestination; // 지금 어디로 향하고 있는가.
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

    // 몬스터가 이동하는 방법.
    // 어디를 갈지 지정한다.
    // 지정된 위치를 갈 때 까지 계속해서 이동한다.
    // 몬스터가 해당 위치에 있다면, 다음 위치를 지정한다.
    // 해당 위치에 정확히 도착할 가능성이 매우 낮으므로, 위치에서 오차범위를 일정 두어 해당 부근에 도착하면 도착한걸로 한다.

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
