using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnergyBarrierGenerator : InteractableBuilding
{
    protected int hpMax;
    protected int hpCurrent;
    public int HpCurrent { get { return hpCurrent; } }

    protected GameObject[] energyBarrierArray;

    protected bool onOff; // on/off시 올라오고 내려가는 연출 like 경찰바리게이트 

    public void SetActiveEnergyBarrier()  // 현재 On인지 Off인제 
    {
        for (int i = 0; i < energyBarrierArray.Length; i++)
        {
            energyBarrierArray[i].SetActive(onOff);
        }
    }
    public void TakeDamage(int damage)
    {
        hpCurrent -= damage;
        if (hpCurrent <= 0)
        {
            onOff = false;
            SetActiveEnergyBarrier();
        }
        Debug.Log($"{HpCurrent} / {gameObject.name}");
    }

    public void RepairBarrier()
    {
        hpCurrent += 1;
        if (hpCurrent >= hpMax)
        {
            hpCurrent = hpMax;
            onOff = true;
            SetActiveEnergyBarrier();
        }
    }



    protected override void Initialize()
    {
        onOff = true;
        hpMax = 3;
        hpCurrent = hpMax;
        energyBarrierArray = GameObject.FindGameObjectsWithTag("EnergyBarrier");

        SetActiveEnergyBarrier();
    }

    public override Interaction InteractionStart(Player player)
    {
        if (!onOff) // 에너지방벽이 고장났다면
        {
            return Interaction.Repair;
        }
        else
        {
            return Interaction.None;
        }
    }

    public override float InteractionUpdate(float deltaTime, Interaction interaction)
    {
        RepairBarrier();
        return default;
    }
}
