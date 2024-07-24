using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingManager : Manager
{
    protected List<Building> buildings = new List<Building>();
    public List<Building> Buildings => buildings;
    public Vector2[] roadData = new Vector2[8]; // ������ �迭.
    // �������� �˸� �ϳ��� �������� ���� �������� ������ �ش� ���� ��.
    protected GameObject[] roads;

    public override IEnumerator Initiate()
    {
        roads = GameObject.FindGameObjectsWithTag("Road");
        roadData[0] = new Vector2(95, 0); // ������
        roadData[1] = new Vector2(50, 0); // ���� �������� y 40, ���� ��ġ�� : (50,0.1,20), ������ : (1,1,5)
        roadData[2] = new Vector2(50, 40); // ���� �������� x -40, ���� ��ġ�� : (30, 0.1, 40), ������ : (5,1,1)
        roadData[3] = new Vector2(10, 40); // ���� �������� y -90, ���� ��ġ�� : (10, 0.1, -5), ������ : (1,1,10)
        roadData[4] = new Vector2(10, -50);
        roadData[5] = new Vector2(-30, -50);
        roadData[6] = new Vector2(-30, 0);
        roadData[7] = new Vector2(-75, 0); // ��������
        for (int i = 0; i<roads.Length; i++)
        {   
            roads[i].transform.localScale = RoadScale(roadData[i], roadData[i + 1]);
            roads[i].transform.position =  RoadPosition(roadData[i], roadData[i + 1], i);
            
            //Debug.Log($"{roads[i].transform.localScale}, {roads[i].transform.position}");
            
        }
        yield return null;
    }

    public Vector3 RoadScale(Vector2 start, Vector2 end)
    {
        Vector3 result;
        Vector2 delta = end - start;
        float deltaScale = (Mathf.Abs(delta.x + delta.y) + 10) / 10;
        if(delta.x != 0)
        {
            result = new Vector3(deltaScale,1,1);
        }
        else
        {
            result = new Vector3(1, 1, deltaScale);
        }
        return result;
    }
    public Vector3 RoadPosition(Vector2 start, Vector2 end, int numb)
    {
        Vector3 result;
        Vector2 lengthSize = end - start;
        if(numb %2 == 0)
        {
            result = new Vector3(start.x+lengthSize.x*0.5f, 0.1f, start.y);
        }
        else
        {
            result = new Vector3(start.x, 0.1f, start.y + lengthSize.y*0.5f);

        }

        return result;
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
