using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public abstract class Character : MyComponent
{
    [SerializeField] protected Rigidbody rb;

    public System.Action<string> AnimTrigger;
    public System.Action<string, float> AnimFloat;
    public System.Action<string, int> AnimInt;
    public System.Action<string, bool> AnimBool;

    protected int hpMax;
    protected int hpCurrent;
    public int HpCurrent => hpCurrent;
    protected int attackDamage;
    protected float attackSpeed;
    [SerializeField] protected float moveSpeed = 5;
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
