using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Building
{
    //public override bool CheckBuild() { return default; } // buildPos는 건설하는 타워의 왼쪽아래
    // tiledBuildingPositionLast = 알아서 정해짐.
    // 사다리의 위치 = float 형. 플로트로 계산이 될것.

    protected override void Initialize()
    {

    }

    public override bool CheckBuild()  // buildPos는 건설하는 타워의 중앙값
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
