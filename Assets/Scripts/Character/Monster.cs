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

    public override void Attack(Character target) { }

    bool isRelease = false;

    public MonsterDestroyFunction destroyFunction;
    protected override void MyStart()
    {
        hpMax = 5;
        hpCurrent = 5;
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

    public void ReleaseMonster()
    {
        isRelease= true;
        //NavMesh.SetAreaCost
    }
}
