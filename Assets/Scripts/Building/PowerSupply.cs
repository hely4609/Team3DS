using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSupply : Building
{
    protected int powerMax;
    public int PowerMax
    {
        get; 
        protected set;
    }

    protected int powerCurrent;
    public int PowerCurrent
    {
        get;
        protected set;
    }
    
    protected int level;
    protected int expCurrent; // ���� ��Ƽ� Ore�� ������ .. ������..
    protected int expMax;

}
