using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Building
{
    protected int cost;
    protected int powerConsumption;

    protected int attackDamage;
    protected float attackSpeed;
    protected float attackRange;

    protected Monster target;
    protected bool onOff;

    public void Attack() { }
    public void LockOn() { } // SphereCast
}
