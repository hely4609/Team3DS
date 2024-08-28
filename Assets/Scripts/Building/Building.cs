using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

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
    public BuildingEnum Type { get { return type; } }

    protected bool isNeedLine; // 전선이 필요한가?

    [SerializeField] protected float buildingTimeMax; // 제작에 얼마나 걸리나

    [Networked, SerializeField] public float BuildingTimeCurrent { get; set; } // 얼마나 제작했나

    //protected float completePercent; //(0~1) 제작한 퍼센트

    protected float heightMax;
    protected float heightMin;
    public float CompletePercent
    {
        get { return BuildingTimeCurrent / buildingTimeMax; }
        set { BuildingTimeCurrent = buildingTimeMax * value; }
    }
    // 10%로 하라. 라고 들어옴.
    [SerializeField] protected Material completeMat;
    [SerializeField] protected MeshRenderer[] meshes;
    [SerializeField] protected Collider[] cols;

    [Networked, SerializeField] protected bool IsFixed { get; set; } = false;
    [Networked] float Buildable { get; set; }
    [Networked] protected bool isBuildable { get; set; } = true; // 이 장소에 건설할 수 있나
    protected ChangeDetector _changeDetector;
    protected Vector2Int tiledBuildingPositionCurrent; // 건설하고싶은 현재 위치. 
    [SerializeField] protected Vector2Int tiledBuildingPositionLast; // 건설하고자하는 마지막 위치.
    public Vector2Int TiledBuildingPos { get { return tiledBuildingPositionLast; } set { tiledBuildingPositionLast = value; } } // 임시 코드
    // 건설 매커니즘.
    // tiledBuildingPositionLast는 생성할려고한다 하는 처음 위치.
    // 그러다가 움직이면, float형을 받아오고, 반올림을 했을 때, tiledBuildingPosiotionLast 와 다르면 그때 CheckBuild를 실행

    [SerializeField] protected Vector2Int startPos; // 시작될 포지션. 건물의 중앙값
    public Vector2Int StartPos { get { return startPos; } }
    [SerializeField] protected Vector2Int size; // 사이즈. 건물의 xy 크기
    public Vector2Int BuildingSize { get { return size; } }

    [SerializeField] protected GameObject marker_designed;
    [SerializeField] protected GameObject marker_on;
    [SerializeField] protected GameObject marker_off;

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        Initialize();

        marker_designed.SetActive(false);
        marker_on.SetActive(false);
        marker_off.SetActive(false);

        Debug.Log($"BTC {BuildingTimeCurrent}, btm : {buildingTimeMax}, cp : {CompletePercent}");
        if (IsFixed)
        {
            foreach (var col in cols)
            {
                col.enabled = true;
            }
        }
        else
        {
            foreach (var col in cols)
            {
                col.enabled = false;
            }
        }
        if (CompletePercent < 1)
        {
            foreach (var r in meshes)
            {
                r.material = ResourceManager.Get(ResourceEnum.Material.Buildable);
                r.material.SetFloat("_CompletePercent", CompletePercent);
            }
            marker_designed.SetActive(true);
        }
        else
        {
            foreach (MeshRenderer r in meshes)
            {
                r.material = completeMat;
            }
            marker_on.SetActive(true);
        }
        HeightCheck();

        CheckBuild();
        VisualizeBuildable();
    }

    protected override void MyStart()
    {



    }
    protected abstract void Initialize(); // 건물의 Enum 값 지정해줘야함.


    public virtual bool CheckBuild()  // buildPos는 건설하는 타워의 중앙값
    {
        isBuildable = true;
        List<Building> buildingList = GameManager.Instance.BuildingManager.Buildings;
        if (buildingList.Count > 0)
        {
            foreach (Building building in buildingList)
            {
                Vector2Int distance = building.startPos - tiledBuildingPositionLast;
                Vector2Int sizeSum = (building.size + size + Vector2Int.one) / 2;

                if (building.Type == BuildingEnum.Bridge)
                {
                    if(!(isBuildable = BridgeCheck(building, tiledBuildingPositionLast)))
                    {
                        break;
                    }
                }
                else
                {
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
        }
        return isBuildable;
    }

    protected virtual bool BridgeCheck(Building building, Vector2Int thisBuildingPos)
    {
        bool isBuildable;
        Vector2Int buildingPosRight = Vector2Int.zero;
        Vector2Int buildingPosLeft = Vector2Int.zero;
        
        //Debug.Log($"size : {size}, stairPosRight : {stairPosRight}, stairPosLeft : {stairPosLeft}, railLength : {railLength}, originToMid = {originToMid}, fullSize : {fullSize}");


        if (building.transform.rotation.eulerAngles.y == 0f)
        {
            buildingPosRight = building.startPos + Vector2Int.down * 2;
            buildingPosLeft = building.startPos + new Vector2Int(0, 12);
            size = new Vector2Int(2, 4);

        }
        else if (building.transform.rotation.eulerAngles.y == 90f)
        {
            buildingPosRight = building.startPos + Vector2Int.left * 2;
            buildingPosLeft = building.startPos + new Vector2Int(12, 0);
            size = new Vector2Int(4, 2);
        }
        else if (building.transform.rotation.eulerAngles.y == 180f)
        {
            buildingPosRight = building.startPos + Vector2Int.up * 2;
            buildingPosLeft = building.startPos + new Vector2Int(0, -12);
            size = new Vector2Int(2, 4);

        }
        else if (building.transform.rotation.eulerAngles.y == 270f)
        {
            buildingPosRight = building.startPos + Vector2Int.right * 2;
            buildingPosLeft = building.startPos + new Vector2Int(-12, 0);
            size = new Vector2Int(4, 2);
        }

        Vector2Int distance = buildingPosRight - thisBuildingPos;
        Vector2Int sizeSum = (building.size + size + Vector2Int.one) / 2;

        if (Mathf.Abs(distance.x) >= sizeSum.x || Mathf.Abs(distance.y) >= sizeSum.y)
        {
            isBuildable = true;
        }
        else
        {
            return false;
        }

        distance = buildingPosLeft - thisBuildingPos;
        if (Mathf.Abs(distance.x) >= sizeSum.x || Mathf.Abs(distance.y) >= sizeSum.y)
        {
            isBuildable = true;
        }
        else
        {
            return false;
        }

        return isBuildable;
    }


    public void VisualizeBuildable() // 건설 가능한지 화면에 표시함.
    {
        if (isBuildable)
        {
            foreach (var r in meshes)
            {
                r.material.SetFloat("_Buildable", 1f);
            }
        }
        else
        {
            foreach (var r in meshes)
            {
                r.material.SetFloat("_Buildable", 0f);
            }
        }

    }
    public virtual bool FixPlace() // 건설완료
    {
        startPos = tiledBuildingPositionLast;
        if (isBuildable)
        {
            GameManager.Instance.BuildingManager.AddBuilding(this);
            IsFixed = true;
            //HeightCheck();

            return true;
        }
        else
        {
            return false;
        }
    }
    public virtual void BuildBuilding(float deltaTime)
    {
        // 마우스를 누르고 있으면 점점 수치가 차오름.
        // 델타 타임 만큼 자신의 buildingTimeCurrent를 올림.
        if (CompletePercent < 1)
        {
            BuildingTimeCurrent += deltaTime * 100;
        }
      
    }

    // 건물의 높이를 측정하는 함수.
    // 건물을 지을 때 진행도에 따라 차오르는것을 표현하기 위함.
    protected void HeightCheck()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            i++;
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);

        heightMax = mesh.bounds.max.y;
        heightMin = mesh.bounds.min.y;
        //Debug.Log(mesh.bounds.max.y);
        //Debug.Log(mesh.bounds.min.y);

        //Debug.Log(mesh.bounds.max.y + Mathf.Abs(mesh.bounds.min.y));

        foreach (MeshRenderer r in meshes)
        {
            r.material.SetFloat("_HeightMax", heightMax);
            r.material.SetFloat("_HeightMin", heightMin);
        }

    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(isBuildable):
                    VisualizeBuildable();
                    break;


                case nameof(IsFixed):
                    Debug.Log(cols.Length);
                    foreach (Collider col in cols)
                    {
                        col.enabled = true;
                    }
                    break;

                case nameof(BuildingTimeCurrent):
                    {
                        foreach (MeshRenderer r in meshes)
                        {
                            r.material.SetFloat("_CompletePercent", CompletePercent);
                        }

                        if (CompletePercent >= 1)
                        {
                            foreach (MeshRenderer r in meshes)
                                r.material = completeMat;
                            marker_designed.SetActive(false);
                            marker_on.SetActive(true);
                        }
                    }
                    break;

            }

        }
    }
}
