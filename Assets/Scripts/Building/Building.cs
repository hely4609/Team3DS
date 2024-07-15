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
    protected BuildingEnum type; // Ÿ�� ����
    protected bool isNeedLine; // ������ �ʿ��Ѱ�?

    protected float buildingTimeMax; // ���ۿ� �󸶳� �ɸ���
    protected float buildingTimeCurrent; // �󸶳� �����߳�
    protected float completePercent; //(0~1) ������ �ۼ�Ʈ
    public float CompletePercent { get; set; }

    [SerializeField] protected MeshRenderer[] mesh;
    [SerializeField] protected Collider col;

    [SerializeField] protected bool isBuildable; // �� ��ҿ� �Ǽ��� �� �ֳ�
    protected Vector2Int tiledBuildingPositionCurrent; // �Ǽ��ϰ���� ���� ��ġ. 
    [SerializeField] protected Vector2Int tiledBuildingPositionLast; // �Ǽ��ϰ����ϴ� ������ ��ġ.
    public Vector2Int TiledBuildingPos { get { return tiledBuildingPositionLast; } set { tiledBuildingPositionLast = value; } } // �ӽ� �ڵ�
    // �Ǽ� ��Ŀ����.
    // tiledBuildingPositionLast�� �����ҷ����Ѵ� �ϴ� ó�� ��ġ.
    // �׷��ٰ� �����̸�, float���� �޾ƿ���, �ݿø��� ���� ��, tiledBuildingPosiotionLast �� �ٸ��� �׶� CheckBuild�� ����

    [SerializeField] protected Vector2Int startPos; // ���۵� ������. �ǹ��� �߾Ӱ�
    [SerializeField] protected Vector2Int size; // ������. �ǹ��� xy ũ��
    protected override void MyStart()
    {
        Initialize();
    }
    protected abstract void Initialize(); // �ǹ��� Enum �� �����������.
    public virtual bool CheckBuild()  // buildPos�� �Ǽ��ϴ� Ÿ���� ���ʾƷ�
    {
        isBuildable = CheckAlreadyBuild();
        
        VisualizeBuildable();
        return isBuildable;
    }
    public virtual bool CheckAlreadyBuild() // �Ǽ��Ϸ��� �ǹ��� �ٸ� �ǹ��� ���ƴ��� üũ.
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

    public void VisualizeBuildable() // �Ǽ� �������� ȭ�鿡 ǥ����.
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
            Debug.Log("�ȵ�");
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
    } // ��ġ�� ������.
}
