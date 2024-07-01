using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : Manager
{
    protected List<Building> buildings = new List<Building>();
    public List<Building> Buildings => buildings;

    public override IEnumerator Initiate() { yield return null; }

    public void AddBuilding(Building addedBuilding) // �ǹ��� ���� �Ǽ��ߴ�.
    {
        buildings.Add(addedBuilding);
    }
    public void RemoveBuilding(Building removedBuilding) // �ǹ��� ���ݴ�(�ʿ��Ѱ�? �ϴ� �׷��� ������)
    {
        buildings.Remove(removedBuilding);
    }
    public void ChangeBuildingList(List<Building> newBuildingList) // �ǹ� ����Ʈ�� �����Ѵ�. (��Ʈ��ũ ���� �� �ٽ� ����Ǿ����� ��ġ)
    {
        buildings= newBuildingList;
    }

}
