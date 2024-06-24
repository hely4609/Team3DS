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
    protected BuildingEnum type; // 타워 종류
    protected bool isNeedLine; // 전선이 필요한가?

    protected float buildingTimeMax; // 제작에 얼마나 걸리나
    protected float buildingTimeCurrent; // 얼마나 제작했나
    protected float completePercent; //(0~1) 제작한 퍼센트
    public float CompletePercent {get; set; } 

    protected bool isBuildable; // 이 장소에 건설할 수 있나
    protected Vector2Int tiledBuildingPositionCurrent; // 건설하고싶은 현재 위치. 
    protected Vector2Int tiledBuildingPositionLast; // 건설하고자하는 마지막 위치.

    protected Vector2Int startPos; // 시작될 포지션. 건물의 왼쪽 아래 지점
    protected Vector2Int size; // 사이즈. 건물의 xy 크기

    public virtual bool CheckBuild() { return default; } // buildPos는 건설하는 타워의 왼쪽아래
    public virtual bool FixPlace() { return default; } // 위치를 고정함.
}
