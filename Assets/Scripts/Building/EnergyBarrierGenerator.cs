using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBarrierGenerator : Building
{
    protected int hpMax;
    protected int hpCurrent;

    protected GameObject[] energyBarrierArray;

    protected bool onOff; // on/off�� �ö���� �������� ���� like �����ٸ�����Ʈ 

    public void SetActiveEnergyBarrier() { }
    public void TakeDamage() {}
}
