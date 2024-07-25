using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingManager : Manager
{
    protected List<Building> buildings = new List<Building>();
    public List<Building> Buildings => buildings;
    public List<Vector2> roadData = new List<Vector2>(); // ������ ����Ʈ
    // �������� �˸� �ϳ��� �������� ���� �������� ������ �ش� ���� ��.
    protected List<GameObject> corners = new List<GameObject>();
    protected List<GameObject> roads = new List<GameObject>();

    public override IEnumerator Initiate()
    {
        // ����Ʈ�� �����, ������ �������� ����.
        // ���� ���� �ڿ��� ��� ���� Ȯ��� �����̶� �߰��Ǳ� ���ؼ��� �� ������ ���Ұ����� ����.
        roadData.Add(new Vector2(-75, 0)); // ��������. ������ ������ ������ �������� ����.
        roadData.Add(new Vector2(-30, 0));
        roadData.Add(new Vector2(-30, -50));
        roadData.Add(new Vector2(10, -50));
        roadData.Add(new Vector2(10, 40)); // ���� �������� y -90, ���� ��ġ�� : (10, 0.1, -5), ������ : (1,1,10)
        roadData.Add(new Vector2(50, 40)); // ���� �������� x -40, ���� ��ġ�� : (30, 0.1, 40), ������ : (5,1,1)
        roadData.Add(new Vector2(50, 0)); // ���� �������� y 40, ���� ��ġ�� : (50,0.1,20), ������ : (1,1,5)
        roadData.Add(new Vector2(95, 0)); // ������

        for (int i = 0; i < roadData.Count; i++)
        {
            if (i == 0 || i == roadData.Count - 1)
            {
                corners.Add(RoadInstantiate());
                corners[i].transform.position = new Vector3(roadData[i].x, 0.1f, roadData[i].y);
            }
            else
            {
                corners.Add(CornerInstantiate());
                corners[i].transform.position = new Vector3(roadData[i].x, 0.1f, roadData[i].y);
                corners[i].transform.rotation = Quaternion.Euler(CornerRotation(roadData[i - 1], roadData[i], roadData[i + 1]));
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
                roads[j].transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

            }
            else if (Mathf.Abs(roadVector.x) > 0)
            {
                roads.Add(RoadInstantiate());
                roads[j].transform.position = RoadPosition(roadData[j], roadData[j + 1]);
                roads[j].transform.localScale = RoadScale(roadData[j], roadData[j+1]);
                roads[j].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }

        yield return null;
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
