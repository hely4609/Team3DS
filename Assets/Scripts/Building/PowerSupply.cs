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
    protected int expCurrent; // 몹을 잡아서 Ore를 넣으면 .. 레벨업..
    protected int expMax;

}
