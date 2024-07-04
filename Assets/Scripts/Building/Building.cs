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
    protected BuildingEnum type; // Ÿ�� ����
    protected bool isNeedLine; // ������ �ʿ��Ѱ�?

    protected float buildingTimeMax; // ���ۿ� �󸶳� �ɸ���
    protected float buildingTimeCurrent; // �󸶳� �����߳�
    protected float completePercent; //(0~1) ������ �ۼ�Ʈ
    public float CompletePercent { get; set; }

    [SerializeField] protected bool isBuildable; // �� ��ҿ� �Ǽ��� �� �ֳ�
    protected Vector2Int tiledBuildingPositionCurrent; // �Ǽ��ϰ���� ���� ��ġ. 
    [SerializeField] protected Vector2Int tiledBuildingPositionLast; // �Ǽ��ϰ����ϴ� ������ ��ġ.
    public Vector2Int TiledBuildingPos { get { return tiledBuildingPositionLast; } set { tiledBuildingPositionLast = value; } } // �ӽ� �ڵ�
    // �Ǽ� ��Ŀ����.
    // tiledBuildingPositionLast�� �����ҷ����Ѵ� �ϴ� ó�� ��ġ.
    // �׷��ٰ� �����̸�, float���� �޾ƿ���, �ݿø��� ���� ��, tiledBuildingPosiotionLast �� �ٸ��� �׶� CheckBuild�� ����

    [SerializeField] protected Vector2Int startPos; // ���۵� ������. �ǹ��� ���� �Ʒ� ����
    [SerializeField] protected Vector2Int size; // ������. �ǹ��� xy ũ��
    protected override void MyStart()
    {
        Initialize();
    }
    protected abstract void Initialize(); // �ǹ��� Enum �� �����������.
    public virtual bool CheckBuild()  // buildPos�� �Ǽ��ϴ� Ÿ���� ���ʾƷ�
    {
        isBuildable = true;
        List<Building> buildingList = GameManager.Instance.BuildingManager.Buildings;
        Debug.Log($"{buildingList.Count}");
        //List<Building> buildingList = BuildingManager.Buildings; // �ӽ� �ڵ�
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
            Debug.Log("�ȵ�");
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
    } // ��ġ�� ������.
}
