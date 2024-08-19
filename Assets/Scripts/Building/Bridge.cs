using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Building
{
    //public override bool CheckBuild() { return default; } // buildPos�� �Ǽ��ϴ� Ÿ���� ���ʾƷ�
    // tiledBuildingPositionLast = �˾Ƽ� ������.
    // ��ٸ��� ��ġ = float ��. �÷�Ʈ�� ����� �ɰ�.

    protected override void Initialize()
    {

    }

    public override bool CheckBuild()  // buildPos�� �Ǽ��ϴ� Ÿ���� �߾Ӱ�
    {
        isBuildable = true;
        List<Building> buildingList = GameManager.Instance.BuildingManager.Buildings;
        if (buildingList.Count > 0)
        {
            foreach (Building building in buildingList)
            {
                Vector2Int distance = building.StartPos - tiledBuildingPositionLast;
                Vector2Int sizeSum = (building.buildingSize + size + Vector2Int.one) / 2;
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
}
