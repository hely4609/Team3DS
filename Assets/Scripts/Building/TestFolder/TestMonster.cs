using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public delegate void MonsterDestroyFunction(TestMonster monster);

public class TestMonster : Monster
{

    bool isRelease = false;

    //public MonsterDestroyFunction destroyFunction;
    protected override void MyStart()
    {
        hpMax = 5;
        hpCurrent = 5;
    }


    
    public override int TakeDamage(Tower attacker, int damage)
    {
        hpCurrent -= damage;
        if(hpCurrent <= 0)
        {
            destroyFunction.Invoke(this);
            Destroy(gameObject);
        }
        Debug.Log($"{HpCurrent} / {gameObject.name}");
        return 0;
    }

}
