using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : InteractableBuilding
{
    //public override bool CheckBuild() { return default; } // buildPos는 건설하는 타워의 왼쪽아래
    // tiledBuildingPositionLast = 알아서 정해짐.
    // 사다리의 위치 = float 형. 플로트로 계산이 될것.

    protected override void Initialize()
    {
        type = BuildingEnum.Bridge;
        isNeedLine = false;
        size = new Vector2Int(2, 4);
        buildingTimeMax = 10;

    }

    public override bool CheckBuild()  // buildPos는 건설하는 타워의 중앙값
    {
        isBuildable = true;
        List<Building> buildingList = GameManager.Instance.BuildingManager.Buildings;
        Vector2Int stairPosRight = tiledBuildingPositionLast;
        Vector2Int stairPosLeft = tiledBuildingPositionLast + new Vector2Int(0, 10);

        if (buildingList.Count > 0)
        {
            foreach (Building building in buildingList)
            {
                Vector2Int distance = building.StartPos - stairPosRight;
                Vector2Int sizeSum = (building.BuildingSize + size + Vector2Int.one) / 2;
                if (Mathf.Abs(distance.x) >= sizeSum.x || Mathf.Abs(distance.y) >= sizeSum.y)
                {
                    isBuildable = true;
                }
                else
                {
                    isBuildable = false;
                    break;
                }

                distance = building.StartPos - stairPosLeft;
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
    public override bool FixPlace() // 건설완료
    {
        Debug.Log("hey");
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
}
