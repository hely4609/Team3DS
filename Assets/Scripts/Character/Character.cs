using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public abstract class Character : MyComponent
{
    protected Rigidbody rb;

    protected int hpMax;
    protected int hpCurrent;
    public int HpCurrent => hpCurrent;
    protected int attackDamage;
    protected float attackSpeed;
    protected float moveSpeed = 10;
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    
    public virtual void Move(Vector3 direction) { }
    public virtual void MoveToDestination(Vector3 destination) { }
    public virtual int TakeDamage(Character attacker, int damage) { return default; }
    public virtual int TakeDamage(Tower attacker, int damage) { return default; }
    public virtual void Attack(Character target) { }

    
}
