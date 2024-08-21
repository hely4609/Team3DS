using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

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
        name = "다리";

    }

    public override bool CheckBuild()  // buildPos는 건설하는 타워의 중앙값
    {
        isBuildable = true;
        List<Building> buildingList = GameManager.Instance.BuildingManager.Buildings;
        Vector2Int stairPosRight = tiledBuildingPositionLast + Vector2Int.down * 2;
        Vector2Int stairPosLeft = tiledBuildingPositionLast + new Vector2Int(0, 12);

        if (buildingList.Count > 0)
        {
            foreach (Building building in buildingList)
            {
                Vector2Int distance = building.StartPos - stairPosRight;
                Vector2Int sizeSum = (building.BuildingSize + size) / 2;
                if (building.Type == BuildingEnum.Bridge)
                {
                    isBuildable = BridgeCheck(building, stairPosRight);
                    if (!isBuildable)
                    {
                        return false;
                    }
                    isBuildable = BridgeCheck(building, stairPosLeft);
                    if (!isBuildable)
                    {
                        return false;
                    }

                    Vector2Int buildingPosMid = building.StartPos + new Vector2Int(0, 7);
                    Vector2Int thisPosMid = tiledBuildingPositionLast + new Vector2Int(0, 7);
                    Vector2Int fullSize = new Vector2Int(2, 20);
                    distance = buildingPosMid - thisPosMid;

                    sizeSum = (building.BuildingSize + fullSize) / 2;
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
        }
        return isBuildable;
    }

    protected override bool BridgeCheck(Building building, Vector2Int thisBuildingPos)
    {
        bool isBuildable;
        Vector2Int buildingPosRight = building.StartPos + Vector2Int.down * 2;
        Vector2Int buildingPosLeft = building.StartPos + new Vector2Int(0, 12);
        Vector2Int distance = buildingPosRight - thisBuildingPos;
        Vector2Int sizeSum = (building.BuildingSize + size + Vector2Int.one) / 2;

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
    public override bool FixPlace() // 건설완료
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
                            foreach (Collider col in cols)
                                col.isTrigger = false;
                        }
                    }
                    break;

            }

        }
    }
}
