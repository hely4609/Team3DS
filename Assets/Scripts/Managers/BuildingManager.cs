using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : Manager
{
    protected List<Building> buildings = new List<Building>();
    public List<Building> Buildings => buildings;

    public override IEnumerator Initiate() { yield return null; }

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
        buildings= newBuildingList;
    }

}
