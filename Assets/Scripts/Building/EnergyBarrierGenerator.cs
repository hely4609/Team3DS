using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBarrierGenerator : Building
{
    protected int hpMax;
    protected int hpCurrent;

    protected GameObject[] energyBarrierArray;

    protected bool onOff; // on/off시 올라오고 내려가는 연출 like 경찰바리게이트 

    public void SetActiveEnergyBarrier() { }
    public void TakeDamage() {}
}
