using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
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

    [SerializeField] protected MeshRenderer[] mesh;
    [SerializeField] protected Collider col;

    [SerializeField] protected bool isBuildable; // 이 장소에 건설할 수 있나
    protected Vector2Int tiledBuildingPositionCurrent; // 건설하고싶은 현재 위치. 
    [SerializeField] protected Vector2Int tiledBuildingPositionLast; // 건설하고자하는 마지막 위치.
    public Vector2Int TiledBuildingPos { get { return tiledBuildingPositionLast; } set { tiledBuildingPositionLast = value; } } // 임시 코드
    // 건설 매커니즘.
    // tiledBuildingPositionLast는 생성할려고한다 하는 처음 위치.
    // 그러다가 움직이면, float형을 받아오고, 반올림을 했을 때, tiledBuildingPosiotionLast 와 다르면 그때 CheckBuild를 실행

    [SerializeField] protected Vector2Int startPos; // 시작될 포지션. 건물의 중앙값
    [SerializeField] protected Vector2Int size; // 사이즈. 건물의 xy 크기
    protected override void MyStart()
    {
        Initialize();
    }
    protected abstract void Initialize(); // 건물의 Enum 값 지정해줘야함.
    public virtual bool CheckBuild()  // buildPos는 건설하는 타워의 왼쪽아래
    {
        isBuildable = CheckAlreadyBuild();
        
        VisualizeBuildable();
        return isBuildable;
    }
    public virtual bool CheckAlreadyBuild() // 건설하려는 건물이 다른 건물에 겹쳤는지 체크.
    {
        isBuildable = true;
        List<Building> buildingList = GameManager.Instance.BuildingManager.Buildings;
        Debug.Log($"{buildingList.Count}");
        if (buildingList.Count > 0)
        {
            foreach (Building building in buildingList)
            {
                Vector2Int distance = building.startPos - tiledBuildingPositionLast;
                Vector2Int sizeSum = (building.size + size + Vector2Int.one) / 2;
                if (Mathf.Abs(distance.x) >= sizeSum.x || Mathf.Abs(distance.y) >= sizeSum.y)
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
        return isBuildable;
    }

    public void VisualizeBuildable() // 건설 가능한지 화면에 표시함.
    { 
        if (isBuildable)
        {
            Debug.Log("OK");
            foreach(MeshRenderer meshes in mesh)
            {
                meshes.material = ResourceManager.Get(ResourceEnum.Material.Buildable);
            }
        }
        else
        {
            Debug.Log("안됨");
            foreach (MeshRenderer meshes in mesh)
            { 
                meshes.material = ResourceManager.Get(ResourceEnum.Material.Buildunable);
            }
        }
    }
    public virtual bool FixPlace()
    {
        startPos = tiledBuildingPositionLast;
        if (CheckBuild())
        {
            GameManager.Instance.BuildingManager.AddBuilding(this);
            col.enabled = true;
            return true;
        }
        else
        {
            return false;
        }
    } // 위치를 고정함.
}
