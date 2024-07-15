using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pylon : Building
{
    protected int cost;
    // protected int powerCurrent;
    protected float powerRange;

    protected override void Initialize()
    {
        // ����Ʈ ��.
        type = BuildingEnum.Pylon;
        buildingTimeMax = 10;
        size = new Vector2Int(1, 1);
        powerRange = 20;
    }

    // �� ���.

}
