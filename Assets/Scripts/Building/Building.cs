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
    public Vector2Int TiledBuildingPos { get { return tiledBuildingPositionLast; } set { tiledBuildingPositionLast = value; } }
    // 건설 매커니즘.
    // tiledBuildingPositionLast는 생성할려고한다 하는 처음 위치.
    // 그러다가 움직이면, float형을 받아오고, 반올림을 했을 때, tiledBuildingPosiotionLast 와 다르면 그때 CheckBuild를 실행

    protected Vector2Int startPos; // 시작될 포지션. 건물의 왼쪽 아래 지점
    protected Vector2Int size; // 사이즈. 건물의 xy 크기
    private void OnEnable()
    {
        Initialize();
    }
    protected abstract void Initialize();
    public virtual bool CheckBuild()  // buildPos는 건설하는 타워의 왼쪽아래
    {
        List<Building> buildingList = GameManager.Instance.BuildingManager.Buildings;
        Vector2Int leftUp = tiledBuildingPositionLast + new Vector2Int(size.x, 0);
        Vector2Int rightDown = tiledBuildingPositionLast + new Vector2Int(0, size.y);
        Vector2Int rightUp = tiledBuildingPositionLast + size;
        Vector2Int[] buildingPoint = {tiledBuildingPositionLast, leftUp, rightDown, rightUp};
        if (buildingList.Count > 0)
        {
            foreach(Building building in buildingList)
            {
                Vector2Int availableStartPos = building.startPos;
                Vector2Int availableEndPos = building.startPos+size;

                for(int i = 0; i< buildingPoint.Length; i++)
                {
                    if (buildingPoint[i].x > availableStartPos.x && buildingPoint[i].x < availableEndPos.x)
                    {
                        if(buildingPoint[i].y > availableStartPos.y && buildingPoint[i].y < availableEndPos.y)
                        {
                            return false;
                        }
                    }    
                }
            }
        }
        return true;
    }
    public virtual bool FixPlace() { return default; } // 위치를 고정함.
}
