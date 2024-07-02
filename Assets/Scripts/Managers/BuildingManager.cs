using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : Manager
{
    protected List<Building> buildings = new List<Building>();
    public List<Building> Buildings => buildings;
    public Vector2[] roadData = new Vector2[8]; // ������ �迭.
    // �������� �˸� �ϳ��� �������� ���� �������� ������ �ش� ���� ��.


    public override IEnumerator Initiate()
    {
        roadData[0] = new Vector2(95, 0); // ������
        roadData[1] = new Vector2(50, 0); // ���� �������� y 40, �濡���� ��ġ�� : (50,0.1,20), ������ : (1,1,5)
        roadData[2] = new Vector2(50, 40); // ���� �������� x -40, �濡���� ��ġ�� : (30, 0.1, 40), ������ : (5,1,1)
        roadData[3] = new Vector2(10, 40);
        roadData[4] = new Vector2(10, -50);
        roadData[5] = new Vector2(-30, -50);
        roadData[6] = new Vector2(-30, 1);
        roadData[7] = new Vector2(-75, 1); // ��������

        yield return null;
    }

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
        buildings = newBuildingList;
    }

}
