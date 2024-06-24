using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingEnum
{
    Tower,
    Pylon,
    Bridge
}

public abstract class Building : MonoBehaviour
{
    protected BuildingEnum type;
    protected bool isNeedLine;

    protected float buildingTimeMax;
    protected float buildingTimeCurrent;
    protected float completePercent; //(0~1)
    public float CompletePercent {get; set; }

    protected bool isBuildable;
    protected Vector2Int tiledBuildingPositionCurrent;
    protected Vector2Int tiledBuildingPositionLast;

    protected Vector2Int startPos;
    protected Vector2Int size;

    public virtual bool CheckBuild() { return default; } // buildPos는 건설하는 타워의 왼쪽아래
    public virtual bool FixPlace() { return default; }
}
