using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBarrier : Building
{
    protected override void Initialize()
    {
        type = BuildingEnum.Barrier;
    }
}
