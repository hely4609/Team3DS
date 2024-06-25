using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected int hpMax;
    protected int hpCurrent;
    protected int attackDamage;
    protected float attackSpeed;
    protected float moveSpeed;


    protected Vector3 preferedDir;
    

    public virtual void Move(Vector3 direction) 
    {
        
    } 
    public virtual void MoveToDestination(Vector3 destination) { }
    public virtual int TakeDamage(Character attacker, int damage) { return default; }
    public virtual int TakeDamage(Tower attacker, int damage) { return default; }
    public virtual void Attack(Character target) { }

    
}
