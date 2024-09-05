using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : Manager
{
    protected List<Building> buildings;
    public List<Building> Buildings => buildings;
    public List<Vector2> roadData; // ������ ����Ʈ
    // �������� �˸� �ϳ��� �������� ���� �������� ������ �ش� ���� ��.
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
        // ����Ʈ�� �����, ������ �������� ����.
        // ���� ���� �ڿ��� ��� ���� Ȯ��� �����̶� �߰��Ǳ� ���ؼ��� �� ������ ���Ұ����� ����.
        roadData.Add(new Vector2(-20, 10));
        roadData.Add(new Vector2(-40, 10)); // ������
        roadData.Add(new Vector2(-40, -40)); // ������
        roadData.Add(new Vector2(40, -40)); // ���� �������� y 40, ���� ��ġ�� : (50,0.1,20), ������ : (1,1,5)
        roadData.Add(new Vector2(40, 50)); // ���� �������� x -40, ���� ��ġ�� : (30, 0.1, 40), ������ : (5,1,1)
        roadData.Add(new Vector2(-75, 50)); // ���� �������� y -90, ���� ��ġ�� : (10, 0.1, -5), ������ : (1,1,10)
        roadData.Add(new Vector2(-75, -75));
        roadData.Add(new Vector2(75, -75));
        roadData.Add(new Vector2(75, 75));
        roadData.Add(new Vector2(-75, 75)); // ��������.

        // �̰� ��� 
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
                    // 10 ���ϴ� ������ �⺻������ ���� 1�� �����Ǿ��ִµ�, �̰� Ÿ�� �������� �� 10�� ũ���̱� ����.
                    // �ؿ��� 90�� ������ ������ ���⵵ z�� x�� �������־����.
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
        // ���簪�� �հ� ���� ���� ���ؼ� x�� ��ȭ�ߴ���, y�� ��ȭ�ߴ��� �ľ�
        // ����ȭ �ؼ� �������ͷ� ����
        Vector2 front = (now - before).normalized;
        Vector2 back = (after - now).normalized;

        if (front == Vector2.right) // x�� Ŀ��
        {
            // �ڴ� �ٲ�°� �޶�������. 
            if (back == Vector2.up) // y�� Ŀ��
            {
                // 0�� ���ư�.
                return Vector3.zero;
            }
            if (back == Vector2.down)
            {
                // 270�� ���ư�
                return new Vector3(0, 270, 0);
            }
        }
        else if (front == Vector2.left) // x�� �۾���
        {
            if (back == Vector2.up) // y�� Ŀ��
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
        else if (front == Vector2.up) // y�� Ŀ��
        {
            if (back == Vector2.right) // x�� Ŀ��
            {
                // 180��
                return new Vector3(0, 180, 0);

            }
            else if (back == Vector2.left) // x�� �۾���
            {
                // 270
                return new Vector3(0, 270, 0);

            }
        }
        else if (front == Vector2.down) // y�� �۾���
        {
            if (back == Vector2.right) // x�� Ŀ��
            {
                // 90��
                return new Vector3(0, 90, 0);

            }
            else if (back == Vector2.left) // x�� �۾���
            {
                // 0
                return Vector3.zero;
            }
        }
        return Vector3.one;
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
