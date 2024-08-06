using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using ResourceEnum;

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
    protected bool isNeedLine; // ������ �ʿ��Ѱ�?

    protected float buildingTimeMax; // ���ۿ� �󸶳� �ɸ���
<<<<<<< HEAD
    [Networked] protected float buildingTimeCurrent { get; set; } // �󸶳� �����߳�
=======
    [Networked] protected float BuildingTimeCurrent { get; set; } // �󸶳� �����߳�
>>>>>>> SubMaster
    //protected float completePercent; //(0~1) ������ �ۼ�Ʈ

    [Networked] protected float HeightMax { get; set; }
    [Networked] protected float HeightMin { get; set; }
    public float CompletePercent { get { return BuildingTimeCurrent / buildingTimeMax; } 
        set { BuildingTimeCurrent = buildingTimeMax * value; } }
    // 10%�� �϶�. ��� ����.
    [SerializeField] protected MeshRenderer[] meshes;
    [SerializeField] protected Collider[] cols;

    [Networked] protected bool IsFixed { get; set; } = false;
    [Networked] protected bool isBuildable{ get; set; } // �� ��ҿ� �Ǽ��� �� �ֳ�
    private ChangeDetector _changeDetector;
    protected Vector2Int tiledBuildingPositionCurrent; // �Ǽ��ϰ���� ���� ��ġ. 
    [SerializeField] protected Vector2Int tiledBuildingPositionLast; // �Ǽ��ϰ����ϴ� ������ ��ġ.
    public Vector2Int TiledBuildingPos { get { return tiledBuildingPositionLast; } set { tiledBuildingPositionLast = value; } } // �ӽ� �ڵ�
    // �Ǽ� ��Ŀ����.
    // tiledBuildingPositionLast�� �����ҷ����Ѵ� �ϴ� ó�� ��ġ.
    // �׷��ٰ� �����̸�, float���� �޾ƿ���, �ݿø��� ���� ��, tiledBuildingPosiotionLast �� �ٸ��� �׶� CheckBuild�� ����

    [SerializeField] protected Vector2Int startPos; // ���۵� ������. �ǹ��� �߾Ӱ�
    [SerializeField] protected Vector2Int size; // ������. �ǹ��� xy ũ��

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        CheckBuild();
    }   
    
    protected override void MyStart()
    {
        //_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        Initialize();
        
        //HeightCheck();
    }
    protected abstract void Initialize(); // �ǹ��� Enum �� �����������.
    public virtual bool CheckBuild()  // buildPos�� �Ǽ��ϴ� Ÿ���� ���ʾƷ�
    {
        isBuildable = CheckAlreadyBuild();
        //Debug.Log(isBuildable);
        //VisualizeBuildable();
        return isBuildable;
    }
    public virtual bool CheckAlreadyBuild() // �Ǽ��Ϸ��� �ǹ��� �ٸ� �ǹ��� ���ƴ��� üũ.
    {
        isBuildable = true;
        List<Building> buildingList = GameManager.Instance.BuildingManager.Buildings;
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

    public void VisualizeBuildable() // �Ǽ� �������� ȭ�鿡 ǥ����.
    { 
        if (isBuildable)
        {
                foreach(MeshRenderer render in meshes)
                {
                    render.material = ResourceManager.Get(ResourceEnum.Material.Buildable);
                }
        }
        else
        {
            foreach (MeshRenderer render in meshes)
            { 
                render.material = ResourceManager.Get(ResourceEnum.Material.Buildunable);
            }
        }
    }
    public virtual bool FixPlace() // �Ǽ��Ϸ�
    {
        Debug.Log("hey");
        startPos = tiledBuildingPositionLast;
        if (CheckBuild())
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
            BuildingTimeCurrent += deltaTime;
        }
        else
        {
            
        }

        // ���콺�� ���� ����. �ٸ� ������ ������ ����.

        // �ϼ��Ǹ� �ϼ��� Material�� �Ѵ�.

        // �Ǽ� �Ϸ�� 
        //foreach (MeshRenderer r in meshes)
        //{
        //    r.material.SetFloat("_CompletePercent", CompletePercent);
        //}

        //if (CompletePercent >= 1)
        //{
        //    foreach (MeshRenderer r in meshes)
        //        r.material = ResourceManager.Get(ResourceEnum.Material.Turret1a);

        //}
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
        
        float max = mesh.bounds.max.y;
        float min = mesh.bounds.min.y;
        //Debug.Log(mesh.bounds.max.y);
        //Debug.Log(mesh.bounds.min.y);

        //Debug.Log(mesh.bounds.max.y + Mathf.Abs(mesh.bounds.min.y));

        foreach (MeshRenderer r in meshes)
        {
            r.material.SetFloat("_HeightMax", max);
            r.material.SetFloat("_HeightMin", min);
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
<<<<<<< HEAD
                case nameof(buildingTimeCurrent):
                    foreach (MeshRenderer r in meshes)
                    {
                        r.material.SetFloat("_CompletePercent", CompletePercent);
                    }

                    if (CompletePercent >= 1)
                    {
                        foreach (MeshRenderer r in meshes)
                            r.material = ResourceManager.Get(ResourceEnum.Material.Turret1a);
                    }
                    break;
=======

                case nameof(IsFixed):
                    Debug.Log(cols.Length);
                    foreach (Collider col in cols)
                    {
                        col.enabled = true;
                    }
                    HeightCheck();
                    break;

                //case nameof(HeightMax):
                //    foreach (MeshRenderer r in meshes)
                //    {
                //        r.material.SetFloat("_HeightMax", HeightMax);
                //    }
                //    break;

                //case nameof(HeightMin):
                //    foreach (MeshRenderer r in meshes)
                //    {
                //        r.material.SetFloat("_HeightMin", HeightMin);
                //    }
                //break;

                case nameof(BuildingTimeCurrent):
                    {
                        foreach (MeshRenderer r in meshes)
                        {
                            r.material.SetFloat("_CompletePercent", CompletePercent);
                        }

                        if (CompletePercent >= 1)
                        {
                            foreach (MeshRenderer r in meshes)
                                r.material = ResourceManager.Get(ResourceEnum.Material.Turret1a);
                        }
                    }
                    break;

>>>>>>> SubMaster
            }

        }
    }
}
