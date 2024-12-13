using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Bridge : InteractableBuilding
{
    //public override bool CheckBuild() { return default; } // buildPos는 건설하는 타워의 왼쪽아래
    // tiledBuildingPositionLast = 알아서 정해짐.
    // 사다리의 위치 = float 형. 플로트로 계산이 될것.

    public override void Spawned()
    {
        base.Spawned();
        if (CompletePercent >= 1)
        {
            foreach (Collider col in cols)
            {
                col.isTrigger = false;
            }

            System.Array.Clear(cols, 0, cols.Length);
        }
    }

    protected override void Initialize()
    {
        cost = 50;

        buildingType = BuildingEnum.Bridge;
        isNeedLine = false;
        size = new Vector2Int(2, 4);
        objectName = "Bridge";
        
    }

    public override bool CheckBuild()
    {
        isBuildable = true;
        List<Building> buildingList = GameManager.Instance.BuildingManager.Buildings;
        Vector2Int stairPosRight = tiledBuildingPositionLast + Vector2Int.down * 2;
        Vector2Int railLength = new Vector2Int(0, 12); // 다리의 걷는부분 길이
        Vector2Int stairPosLeft = tiledBuildingPositionLast + railLength; // 왼쪽 계단의 위치
        Vector2Int originToMid = new Vector2Int(0, 7); // 원점(한쪽 다리에서) 중앙까지의 거리

        Vector2Int fullSize = railLength + new Vector2Int(0, size.y * 2); // 이 다리의 전체 크기
        //Debug.Log($"size : {size}, stairPosRight : {stairPosRight}, stairPosLeft : {stairPosLeft}, railLength : {railLength}, originToMid = {originToMid}, fullSize : {fullSize}");


        if (transform.rotation.eulerAngles.y == 0f)
        {
            stairPosRight = tiledBuildingPositionLast + Vector2Int.down * 2;
            stairPosLeft = tiledBuildingPositionLast + new Vector2Int(0, 12);
            size = new Vector2Int(2, 4);
            fullSize = new Vector2Int(0, 20);

        }
        else if (transform.rotation.eulerAngles.y == 90f)
        {
            stairPosRight = tiledBuildingPositionLast + Vector2Int.left * 2;
            stairPosLeft = tiledBuildingPositionLast + new Vector2Int(12, 0);
            size = new Vector2Int(4, 2);
            fullSize = new Vector2Int(20, 0);
        }
        else if (transform.rotation.eulerAngles.y == 180f)
        {
            stairPosRight = tiledBuildingPositionLast + Vector2Int.up * 2;
            stairPosLeft = tiledBuildingPositionLast + new Vector2Int(0, -12);
            size = new Vector2Int(2, 4);
            fullSize = new Vector2Int(0, 20);
        }
        else if (transform.rotation.eulerAngles.y == 270f)
        {
            stairPosRight = tiledBuildingPositionLast + Vector2Int.right * 2;
            stairPosLeft = tiledBuildingPositionLast + new Vector2Int(-12, 0);
            size = new Vector2Int(4, 2);
            fullSize = new Vector2Int(20, 0);
        }

        //if(90도 돌면, 반전되어야할것)
        //if (transform.rotation.eulerAngles.y == 90)
        //{
        //    size = new Vector2Int(size.y, size.x);
        //    stairPosRight = tiledBuildingPositionLast + Vector2Int.left * 2;
        //    railLength = new Vector2Int(railLength.y, railLength.x);
        //    stairPosLeft = tiledBuildingPositionLast + railLength;
        //    originToMid = new Vector2Int(originToMid.y, originToMid.x);
        //    fullSize = railLength + new Vector2Int(size.x * 2, 0); // 이 다리의 전체 크기
        //    //Debug.Log($"size : {size}, stairPosRight : {stairPosRight}, stairPosLeft : {stairPosLeft}, railLength : {railLength}, originToMid = {originToMid}, fullSize : {fullSize}");
        //}
        //else if (transform.rotation.eulerAngles.y == 180)
        //{
        //    stairPosRight = tiledBuildingPositionLast + Vector2Int.up * 2;
        //    fullSize = railLength + new Vector2Int(0, size.y*2); // 이 다리의 전체 크기
        //    //Debug.Log($"size : {size}, stairPosRight : {stairPosRight}, stairPosLef
        //}
        //else if (transform.rotation.eulerAngles.y == 270)
        //{
        //    size = new Vector2Int(size.y, size.x);
        //    stairPosRight = tiledBuildingPositionLast + Vector2Int.right * 2;
        //    railLength = new Vector2Int(railLength.y, railLength.x);
        //    stairPosLeft = tiledBuildingPositionLast + railLength;
        //    originToMid = new Vector2Int(originToMid.y, originToMid.x);
        //    fullSize = railLength + new Vector2Int(size.x * 2, 0); // 이 다리의 전체 크기
        //    //Debug.Log($"size : {size}, stairPosRight : {stairPosRight}, stairPosLef
        //}


        if (buildingList.Count > 0)
        {
            foreach (Building building in buildingList)
            {
                Vector2Int distance = building.StartPos - stairPosRight;
                Vector2Int sizeSum = (building.BuildingSize + size) / 2;
                if (building.BuildingType == BuildingEnum.Bridge)
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

                    Vector2Int bridgeOriginToMid = new Vector2Int(0, 7);
                    if (building.transform.rotation.eulerAngles.y == 90)
                    {
                        bridgeOriginToMid = new Vector2Int(bridgeOriginToMid.y, bridgeOriginToMid.x);
                    }
                    Vector2Int buildingPosMid = building.StartPos + bridgeOriginToMid;
                    Vector2Int thisPosMid = tiledBuildingPositionLast + originToMid;
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
        Vector2Int railLength = new Vector2Int(0, 12);
        Vector2Int buildingPosRight = building.StartPos + Vector2Int.down * 2;
        Vector2Int buildingPosLeft = building.StartPos + railLength; //여길 고쳐야함.

        if (building.transform.rotation.eulerAngles.y == 0)
        {
            railLength = new Vector2Int(0, 12);
            buildingPosRight = building.StartPos + Vector2Int.down * 2;
            buildingPosLeft = building.StartPos + railLength;
        }

        if (building.transform.rotation.eulerAngles.y == 90)
        {
            railLength = new Vector2Int(12, 0);
            buildingPosRight = building.StartPos + Vector2Int.left * 2;
            buildingPosLeft = building.StartPos + railLength;
        }

        if (building.transform.rotation.eulerAngles.y == 180)
        {
            railLength = new Vector2Int(0, -12);
            buildingPosRight = building.StartPos + Vector2Int.up * 2;
            buildingPosLeft = building.StartPos + railLength;
        }

        if (building.transform.rotation.eulerAngles.y == 270)
        {
            railLength = new Vector2Int(-12, 0);
            buildingPosRight = building.StartPos + Vector2Int.right * 2;
            buildingPosLeft = building.StartPos + railLength;
        }



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

    public override bool InteractionEnd()
    {
        return true;
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

                            marker_designed.SetActive(false);
                            marker_on.SetActive(true);

                            System.Array.Clear(cols, 0, cols.Length);
                        }
                    }
                    break;

            }

        }
    }
}
