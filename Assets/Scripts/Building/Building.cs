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
    protected BuildingEnum type; // Ÿ�� ����
    public BuildingEnum Type { get { return type; } }

    protected bool isNeedLine; // ������ �ʿ��Ѱ�?

    [SerializeField] protected float buildingTimeMax; // ���ۿ� �󸶳� �ɸ���

    [Networked, SerializeField] public float BuildingTimeCurrent { get; set; } // �󸶳� �����߳�

    //protected float completePercent; //(0~1) ������ �ۼ�Ʈ

    protected float heightMax;
    protected float heightMin;
    public float CompletePercent
    {
        get { return BuildingTimeCurrent / buildingTimeMax; }
        set { BuildingTimeCurrent = buildingTimeMax * value; }
    }
    // 10%�� �϶�. ��� ����.
    [SerializeField] protected Material completeMat;
    [SerializeField] protected MeshRenderer[] meshes;
    [SerializeField] protected Collider[] cols;

    [Networked, SerializeField] protected bool IsFixed { get; set; } = false;
    [Networked] float Buildable { get; set; }
    [Networked] protected bool isBuildable { get; set; } = true; // �� ��ҿ� �Ǽ��� �� �ֳ�
    protected ChangeDetector _changeDetector;
    protected Vector2Int tiledBuildingPositionCurrent; // �Ǽ��ϰ���� ���� ��ġ. 
    [SerializeField] protected Vector2Int tiledBuildingPositionLast; // �Ǽ��ϰ����ϴ� ������ ��ġ.
    public Vector2Int TiledBuildingPos { get { return tiledBuildingPositionLast; } set { tiledBuildingPositionLast = value; } } // �ӽ� �ڵ�
    // �Ǽ� ��Ŀ����.
    // tiledBuildingPositionLast�� �����ҷ����Ѵ� �ϴ� ó�� ��ġ.
    // �׷��ٰ� �����̸�, float���� �޾ƿ���, �ݿø��� ���� ��, tiledBuildingPosiotionLast �� �ٸ��� �׶� CheckBuild�� ����

    [SerializeField] protected Vector2Int startPos; // ���۵� ������. �ǹ��� �߾Ӱ�
    public Vector2Int StartPos { get { return startPos; } }
    [SerializeField] protected Vector2Int size; // ������. �ǹ��� xy ũ��
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
    protected abstract void Initialize(); // �ǹ��� Enum �� �����������.


    public virtual bool CheckBuild()  // buildPos�� �Ǽ��ϴ� Ÿ���� �߾Ӱ�
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


    public void VisualizeBuildable() // �Ǽ� �������� ȭ�鿡 ǥ����.
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
    public virtual bool FixPlace() // �Ǽ��Ϸ�
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
        // ���콺�� ������ ������ ���� ��ġ�� ������.
        // ��Ÿ Ÿ�� ��ŭ �ڽ��� buildingTimeCurrent�� �ø�.
        if (CompletePercent < 1)
        {
            BuildingTimeCurrent += deltaTime * 100;
        }
      
    }

    // �ǹ��� ���̸� �����ϴ� �Լ�.
    // �ǹ��� ���� �� ���൵�� ���� �������°��� ǥ���ϱ� ����.
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
