using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : Manager
{
    protected List<Building> buildings;
    public List<Building> Buildings => buildings;
    public List<Vector2> roadData; // 시작점 리스트
    // 시작점만 알면 하나의 시작점과 다음 시작점을 이으면 해당 길이 됨.
    protected List<GameObject> corners;
    protected List<GameObject> roads;
    
    protected List<RopeStruct> ropeStructs;
    public List<RopeStruct> RopeStructs=> ropeStructs;

    public EnergyBarrierGenerator generator;
    public PowerSupply supply;
  

    public override IEnumerator Initiate()
    {
        buildings = new();
        roadData = new();
        corners = new();
        roads = new();
        // 리스트로 만들고, 순서를 역순으로 변경.
        // 길을 만든 뒤에도 계속 길이 확장될 예정이라 추가되기 위해서는 이 구조가 편할것으로 예상.
        roadData.Add(new Vector2(-20, 10));
        roadData.Add(new Vector2(-40, 10)); // 시작점
        roadData.Add(new Vector2(-40, -40)); // 시작점
        roadData.Add(new Vector2(40, -40)); // 다음 지점까지 y 40, 길의 위치값 : (50,0.1,20), 스케일 : (1,1,5)
        roadData.Add(new Vector2(40, 50)); // 다음 지점까지 x -40, 길의 위치값 : (30, 0.1, 40), 스케일 : (5,1,1)
        roadData.Add(new Vector2(-75, 50)); // 다음 지점까지 y -90, 길의 위치값 : (10, 0.1, -5), 스케일 : (1,1,10)
        roadData.Add(new Vector2(-75, -75));
        roadData.Add(new Vector2(75, -75));
        roadData.Add(new Vector2(75, 75));
        roadData.Add(new Vector2(-75, 75)); // 마지막점.

        // 이걸 어디에 
        CreateRoad();
        
        GameManager.ManagerStarts += ManagerStart;

        yield return null;
    }

    public override void ManagerStart()
    {
        NetworkRunner runner = GameManager.Instance.NetworkManager.Runner;
        if (runner.IsServer)
        {
            generator   = runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.EnergyBarrierGenerator), new Vector3(roadData[0].x, 0, roadData[0].y)).GetComponent<EnergyBarrierGenerator>();
            supply      = runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.PowerSupply),            new Vector3(roadData[0].x + 15, 2, roadData[0].y)).GetComponent<PowerSupply>();

        }
        else
        {
            generator   = GameObject.FindObjectOfType<EnergyBarrierGenerator>();
            supply      = GameObject.FindObjectOfType<PowerSupply>();
        }

        Debug.Log(generator);
        Debug.Log(supply);
    }

    public Vector3 RoadScale(Vector2 start, Vector2 end)
    {
        Vector3 result;
        Vector2 delta = end - start;
        float deltaScale = (Mathf.Abs(delta.x + delta.y) - 10) / 10;
        
        result = new Vector3(deltaScale, 1, 1);
       
        return result;
    }
    public Vector3 RoadPosition(Vector2 start, Vector2 end)
    {
        Vector2 lengthSize = (end - start)*0.5f;
        
        return new Vector3(start.x + lengthSize.x, 0.1f, start.y + lengthSize.y);
    }
    protected void CreateRoad()
    {
        for (int i = 0; i < roadData.Count; i++)
        {
            if (i == 0 || i == roadData.Count - 1)
            {
                corners.Add(RoadInstantiate());
                corners[i].transform.position = new Vector3(roadData[i].x, 0.1f, roadData[i].y);
                corners[i].GetComponent<Road>().Size = new Vector2Int(10, 10);
                //corners[i].transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            }
            else
            {
                corners.Add(CornerInstantiate());
                corners[i].transform.position = new Vector3(roadData[i].x, 0.1f, roadData[i].y);
                corners[i].transform.rotation = Quaternion.Euler(CornerRotation(roadData[i - 1], roadData[i], roadData[i + 1]));
                corners[i].GetComponent<Road>().Size = new Vector2Int(10, 10);
            }
        }

        for (int j = 0; j < roadData.Count - 1; j++)
        {
            Vector2 roadVector = roadData[j] - roadData[j + 1];
            if (Mathf.Abs(roadVector.y) > 0)
            {
                roads.Add(RoadInstantiate());
                roads[j].transform.position = RoadPosition(roadData[j], roadData[j + 1]);
                roads[j].transform.localScale = RoadScale(roadData[j], roadData[j + 1]);
                if (roads[j].TryGetComponent(out Road data))
                {
                    // 10 곱하는 이유는 기본단위가 현재 1로 설정되어있는데, 이게 타워 기준으로 딱 10의 크기이기 때문.
                    // 밑에서 90도 돌리기 때문에 여기도 z와 x를 변경해주어야함.
                    data.Size = new Vector2Int((int)(roads[j].transform.localScale.z * 10), (int)(roads[j].transform.localScale.x * 10));
                }
                roads[j].transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            }
            else if (Mathf.Abs(roadVector.x) > 0)
            {
                roads.Add(RoadInstantiate());
                roads[j].transform.position = RoadPosition(roadData[j], roadData[j + 1]);
                roads[j].transform.localScale = RoadScale(roadData[j], roadData[j + 1]);
                if (roads[j].TryGetComponent(out Road data))
                {
                    data.Size = new Vector2Int((int)(roads[j].transform.localScale.x * 10), (int)(roads[j].transform.localScale.z * 10));
                }
                roads[j].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }
        corners[0].gameObject.SetActive(false);
    }
    protected GameObject CornerInstantiate()
    {
        return GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.CornerWithBarrier);
    }
    protected GameObject RoadInstantiate()
    {
        return GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.RoadWithBarrier);
    }
    public Vector3 CornerRotation(Vector2 before, Vector2 now, Vector2 after)
    {
        // 현재값과 앞과 뒤의 차를 구해서 x가 변화했는지, y가 변화했는지 파악
        // 정규화 해서 단위벡터로 변경
        Vector2 front = (now - before).normalized;
        Vector2 back = (after - now).normalized;

        if (front == Vector2.right) // x가 커짐
        {
            // 뒤는 바뀌는게 달라져야함. 
            if (back == Vector2.up) // y가 커짐
            {
                // 0도 돌아감.
                return Vector3.zero;
            }
            if (back == Vector2.down)
            {
                // 270도 돌아감
                return new Vector3(0, 270, 0);
            }
        }
        else if (front == Vector2.left) // x가 작아짐
        {
            if (back == Vector2.up) // y가 커짐
            {
                // 90
                return new Vector3(0, 90, 0);

            }
            if (back == Vector2.down)
            {
                // 180
                return new Vector3(0, 180, 0);

            }
        }
        else if (front == Vector2.up) // y가 커짐
        {
            if (back == Vector2.right) // x가 커짐
            {
                // 180도
                return new Vector3(0, 180, 0);

            }
            else if (back == Vector2.left) // x가 작아짐
            {
                // 270
                return new Vector3(0, 270, 0);

            }
        }
        else if (front == Vector2.down) // y가 작아짐
        {
            if (back == Vector2.right) // x가 커짐
            {
                // 90도
                return new Vector3(0, 90, 0);

            }
            else if (back == Vector2.left) // x가 작아짐
            {
                // 0
                return Vector3.zero;
            }
        }
        return Vector3.one;
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
