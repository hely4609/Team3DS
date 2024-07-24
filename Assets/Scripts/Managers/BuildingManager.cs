using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingManager : Manager
{
    protected List<Building> buildings = new List<Building>();
    public List<Building> Buildings => buildings;
    public Vector2[] roadData = new Vector2[8]; // 시작점 배열.
    // 시작점만 알면 하나의 시작점과 다음 시작점을 이으면 해당 길이 됨.
    protected GameObject[] roads;

    public override IEnumerator Initiate()
    {
        roads = GameObject.FindGameObjectsWithTag("Road");
        roadData[0] = new Vector2(95, 0); // 시작점
        roadData[1] = new Vector2(50, 0); // 다음 지점까지 y 40, 길의 위치값 : (50,0.1,20), 스케일 : (1,1,5)
        roadData[2] = new Vector2(50, 40); // 다음 지점까지 x -40, 길의 위치값 : (30, 0.1, 40), 스케일 : (5,1,1)
        roadData[3] = new Vector2(10, 40); // 다음 지점까지 y -90, 길의 위치값 : (10, 0.1, -5), 스케일 : (1,1,10)
        roadData[4] = new Vector2(10, -50);
        roadData[5] = new Vector2(-30, -50);
        roadData[6] = new Vector2(-30, 0);
        roadData[7] = new Vector2(-75, 0); // 마지막점
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
