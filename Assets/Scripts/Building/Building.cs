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
    protected BuildingEnum type; // Ÿ�� ����
    protected bool isNeedLine; // ������ �ʿ��Ѱ�?

    protected float buildingTimeMax; // ���ۿ� �󸶳� �ɸ���
    protected float buildingTimeCurrent; // �󸶳� �����߳�
    protected float completePercent; //(0~1) ������ �ۼ�Ʈ
    public float CompletePercent {get; set; } 

    protected bool isBuildable; // �� ��ҿ� �Ǽ��� �� �ֳ�
    protected Vector2Int tiledBuildingPositionCurrent; // �Ǽ��ϰ���� ���� ��ġ. 
    protected Vector2Int tiledBuildingPositionLast; // �Ǽ��ϰ����ϴ� ������ ��ġ.
    public Vector2Int TiledBuildingPos { get { return tiledBuildingPositionLast; } set { tiledBuildingPositionLast = value; } }
    // �Ǽ� ��Ŀ����.
    // tiledBuildingPositionLast�� �����ҷ����Ѵ� �ϴ� ó�� ��ġ.
    // �׷��ٰ� �����̸�, float���� �޾ƿ���, �ݿø��� ���� ��, tiledBuildingPosiotionLast �� �ٸ��� �׶� CheckBuild�� ����

    protected Vector2Int startPos; // ���۵� ������. �ǹ��� ���� �Ʒ� ����
    protected Vector2Int size; // ������. �ǹ��� xy ũ��
    private void OnEnable()
    {
        Initialize();
    }
    protected abstract void Initialize();
    public virtual bool CheckBuild()  // buildPos�� �Ǽ��ϴ� Ÿ���� ���ʾƷ�
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
    public virtual bool FixPlace() { return default; } // ��ġ�� ������.
}
