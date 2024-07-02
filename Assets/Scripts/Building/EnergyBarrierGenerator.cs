using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBarrierGenerator : Building
{
    protected int hpMax;
    protected int hpCurrent;
    public int HpCurrent { get { return hpCurrent; } }

    protected GameObject[] energyBarrierArray;

    protected bool onOff; // on/off�� �ö���� �������� ���� like �����ٸ�����Ʈ 

    public void SetActiveEnergyBarrier()  // ���� On���� Off���� 
    { 
        for(int i = 0; i< energyBarrierArray.Length; i++)
        {
            energyBarrierArray[i].SetActive(onOff);
        }
    }
    public void TakeDamage(int damage) 
    {
        hpCurrent -= damage;
        if (hpCurrent <= 0)
        {
            onOff= false;
        }
        Debug.Log($"{HpCurrent} / {gameObject.name}");
    }


    protected override void Initialize()
    {
        onOff = true;
        hpCurrent = hpMax;
        SetActiveEnergyBarrier();
    }
}
