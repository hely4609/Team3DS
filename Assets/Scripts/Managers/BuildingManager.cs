using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : Manager
{
    protected List<Building> buildings = new List<Building>();
    public List<Building> Buildings => buildings;
    public Vector2[] roadData = new Vector2[8]; // 시작점 배열.
    // 시작점만 알면 하나의 시작점과 다음 시작점을 이으면 해당 길이 됨.


    public override IEnumerator Initiate()
    {
        roadData[0] = new Vector2(95, 0); // 시작점
        roadData[1] = new Vector2(50, 0); // 다음 지점까지 y 40, 길에서는 위치값 : (50,0.1,20), 스케일 : (1,1,5)
        roadData[2] = new Vector2(50, 40); // 다음 지점까지 x -40, 길에서의 위치값 : (30, 0.1, 40), 스케일 : (5,1,1)
        roadData[3] = new Vector2(10, 40);
        roadData[4] = new Vector2(10, -50);
        roadData[5] = new Vector2(-30, -50);
        roadData[6] = new Vector2(-30, 1);
        roadData[7] = new Vector2(-75, 1); // 마지막점

        yield return null;
    }

    public void AddBuilding(Building addedBuilding) // 건물을 새로 건설했다.
    {
        buildings.Add(addedBuilding);
    }
    public void RemoveBuilding(Building removedBuilding) // 건물을 없앴다(필요한가? 일단 그래도 만들어둠)
    {
        buildings.Remove(removedBuilding);
    }
    public void ChangeBuildingList(List<Building> newBuildingList) // 건물 리스트를 갱신한다. (네트워크 유실 후 다시 연결되었을때 조치)
    {
        buildings = newBuildingList;
    }

}
