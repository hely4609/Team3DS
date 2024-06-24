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

    protected virtual void Move(Vector3 direction) { } // 1��Ī�̴ϱ� ���⸸ �־ ��������
    protected virtual void MoveToDestination(Vector3 destination) { }
    protected virtual int TakeDamage(Character attacker, int damage) { return default; }
    protected virtual int TakeDamage(Tower attacker, int damage) { return default; }
    protected virtual void Attack(Character target) { }
}
