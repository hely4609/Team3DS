using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
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
    protected Vector3 preferedDir;
        
    private void FixedUpdate()
    {
        transform.Translate(preferedDir * Time.fixedDeltaTime);
    }

    public virtual void Move(Vector3 direction) 
    {
        preferedDir = direction.normalized;
    } 
    public virtual void MoveToDestination(Vector3 destination) { }
    public virtual int TakeDamage(Character attacker, int damage) { return default; }
    public virtual int TakeDamage(Tower attacker, int damage) { return default; }
    public virtual void Attack(Character target) { }

    
}
