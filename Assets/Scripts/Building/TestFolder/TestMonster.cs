using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonster : Monster
{
    protected override void MyStart()
    {
        hpMax = 5;
        hpCurrent = 5;
    }
    public override int TakeDamage(Tower attacker, int damage)
    {
        hpCurrent -= damage;
        return 0;
    }

}
