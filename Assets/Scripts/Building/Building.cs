using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingEnum
{
    Tower,
    Pylon,
    Bridge,
    Barrier
}

public abstract class Building : MyComponent
{
    protected BuildingEnum type; // 타워 종류
    protected bool isNeedLine; // 전선이 필요한가?

    protected float buildingTimeMax; // 제작에 얼마나 걸리나
    protected float buildingTimeCurrent; // 얼마나 제작했나
    protected float completePercent; //(0~1) 제작한 퍼센트
    public float CompletePercent { get; set; }

    [SerializeField] protected bool isBuildable; // 이 장소에 건설할 수 있나
    protected Vector2Int tiledBuildingPositionCurrent; // 건설하고싶은 현재 위치. 
    [SerializeField] protected Vector2Int tiledBuildingPositionLast; // 건설하고자하는 마지막 위치.
    public Vector2Int TiledBuildingPos { get { return tiledBuildingPositionLast; } set { tiledBuildingPositionLast = value; } } // 임시 코드
    // 건설 매커니즘.
    // tiledBuildingPositionLast는 생성할려고한다 하는 처음 위치.
    // 그러다가 움직이면, float형을 받아오고, 반올림을 했을 때, tiledBuildingPosiotionLast 와 다르면 그때 CheckBuild를 실행

    [SerializeField] protected Vector2Int startPos; // 시작될 포지션. 건물의 왼쪽 아래 지점
    [SerializeField] protected Vector2Int size; // 사이즈. 건물의 xy 크기
    protected override void MyStart()
    {
        Initialize();
    }
    protected abstract void Initialize(); // 건물의 Enum 값 지정해줘야함.
    public virtual bool CheckBuild()  // buildPos는 건설하는 타워의 왼쪽아래
    {
        isBuildable = true;
        List<Building> buildingList = GameManager.Instance.BuildingManager.Buildings;
        Debug.Log($"{buildingList.Count}");
        //List<Building> buildingList = BuildingManager.Buildings; // 임시 코드
        Vector2Int rightUp = tiledBuildingPositionLast + size;
        Vector2Int[] buildingPoint = { tiledBuildingPositionLast, rightUp };
        if (buildingList.Count > 0)
        {
            foreach (Building building in buildingList)
            {

                Vector2Int availableStartPos = building.startPos;
                Vector2Int availableEndPos = building.startPos + size;
                Debug.Log($"{availableStartPos} / {buildingPoint[0]}");

                if ((buildingPoint[0].x >= availableEndPos.x || buildingPoint[1].x <= availableStartPos.x) ||
                    (buildingPoint[0].y >= availableEndPos.y || buildingPoint[1].y <= availableStartPos.y))
                {
                    isBuildable = true;
                }
                else
                {
                    isBuildable = false;
                    break;
                }

            }
        }
        if (isBuildable)
        {
            Debug.Log("OK");
            return true;
        }
        else
        {
            Debug.Log("안됨");
            return false;
        }
    }
    public virtual bool FixPlace()
    {
        startPos = tiledBuildingPositionLast;
        if (CheckBuild())
        {
            GameManager.Instance.BuildingManager.AddBuilding(this);
            return true;
        }
        else
        {
            return false;
        }
    } // 위치를 고정함.
}
