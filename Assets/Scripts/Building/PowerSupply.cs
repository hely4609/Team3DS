using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSupply : InteractableBuilding
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

    [SerializeField] protected int level;
    [SerializeField] protected int expCurrent; // ���� ��Ƽ� Ore�� ������ .. ������..
    [SerializeField] protected int expMax;
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
        expMax = 5;

    }

    public override Interaction InteractionStart(Player player)
    {
        if (player != null) // �÷��̾ ���� ������ ��ǰ�� �����̶��
        {
            Deliver(player);
            return Interaction.Deliver;
        }
        else
        {
            return Interaction.None;
        }
    }
    //public bool Interaction(GameObject target) // ��ǰ �ޱ�
    //{
        //if (target.TryGetComponent<Player>(out Player player)) // ��ȣ�ۿ��Ѱ��� �÷��̾��ΰ�?
        //{
            //// �׷��ٸ� bePicked�� �ֳ�?
            //// �װ��� ��ǰ�ϴ� �����ΰ�?
            //// ��ǰ�� ������ ���� 0���� ū��?
            //Debug.Log($"{gameObject.name}�� ��ǰ ��ȣ�ۿ��� �Ͽ����ϴ�.");
            //return true;
        //}

        //return false;
    //}
    public void Deliver(Player player)
    {
        for (int i = 0; i < player?.OreAmount; i++)
        {
            ExpCurrent++;
        }
        ////if(player.BePicked != null)
        //{
        //    // if(player.BePicked == ���ǰ)
        //    {
        //        // expCurrent += ���ǰ.currentOre
        //        // ���ǰ�� ��Ȱ��ȭ.
        //        // player.bePicked = null
        //    }
        //}
    }
}
