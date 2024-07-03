using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool Interaction(GameObject target);
}

public class PowerSupply : Building, IInteractable
{
    protected int powerMax;
    public int PowerMax
    {
        get { return powerMax; }
        set { powerMax = value; }
    }

    protected int powerCurrent;
    public int PowerCurrent
    {
        get { return powerCurrent; }
        set { powerCurrent = value; }
    }

    protected int level;
    protected int expCurrent; // 몹을 잡아서 Ore를 넣으면 .. 레벨업..
    protected int expMax;
    public int ExpCurrent
    {
        get { return expCurrent; }
        set
        {
            expCurrent = value;
            if (expCurrent >= expMax)
            {
                expCurrent = 0;
                expMax *= 2;
                level++;
            }
        }
    }


    protected override void Initialize()
    {
        powerMax = 10;
        PowerCurrent = powerMax;
        expCurrent = 0;
        expMax = 10;
    }

    public bool Interaction(GameObject target) // 납품 받기
    {
        if (target.TryGetComponent<Player>(out Player player)) // 상호작용한것이 플레이어인가?
        {
            // 그렇다면 bePicked가 있나?
            // 그것이 납품하는 물건인가?
            // 납품할 물건의 수가 0보다 큰가?
            Debug.Log($"{gameObject.name}과 납품 상호작용을 하였습니다.");
            return true;
        }

        return false;
    }
}
