using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public struct RopeStruct
{
    public List<NetworkObject> ropeObjects;
    public List<Vector2> ropePositions;
}
public class Pylon : InteractableBuilding
{
    protected int cost;
    // protected int powerCurrent;
    protected float powerRange;
    // ���� ���� ���
    protected RopeStruct ropeStruct;

    protected override void Initialize()
    {
        // ����Ʈ ��.
        type = BuildingEnum.Pylon;
        buildingTimeMax = 10;
        size = new Vector2Int(1, 1);
        powerRange = 20;
    }
    public override Interaction InteractionStart(Player player)
    {
        // �ϼ��� ���� �ȵ�.
        if (CompletePercent < 1)
        {
            return Interaction.Build;
        }
        else
        {
            // ���� ��������.
            return Interaction.takeRope;
        }
    }
    // �� ���.
    public virtual void Rope(Player player)
    {
        // ���������� �� ����� �÷��̾ �����Ұ�.
        // 1. �÷��̾ �����̸� �ش� ��ġ�� ������ ��
        // 2. ������ �򸮸� powerRange�� �׸�ŭ �پ��
        // 3. powerRange <= 0 �� �Ǹ� ��������.
        // 4-1. ���� �� �ִ� �����̶�� �ϸ� ��ġ.(��ġ �� ������ CheckBuild�ҰŶ� ����)
        // 4-2. ���Ű�� ������ ������ �������� �տ� �� ������ ������.

        // ������ �򸮴°� ĳ���Ͱ� ���������� �̵��� �� �� �򸮰�,
        // ĳ���Ϳ� Ÿ�������� �Ÿ����� �Һ�� ������ ������ ����.
        float remainRope = powerRange;
        Vector3 pos = (transform.position - player.transform.position);
        remainRope -= Mathf.Sqrt(Mathf.Pow(pos.x, 2) + Mathf.Pow(pos.y, 2));
        
    }
    public void SetRope(Vector3Int beforePos, Vector3Int nowPos)
    {
        // ���� ��ġ ���� ���� ��ġ�� �޾ƿ���, �� ���̿� ���� ������.
        GameObject ropePrefab = GameManager.Instance.PoolManager.Instantiate(ResourceEnum.Prefab.Rope);
        ropePrefab.transform.position = (Vector3)(nowPos - beforePos)*0.5f + (Vector3)(beforePos);
    }
    //public void ResetRope()
    public Vector3 RopePosition(Vector2 start, Vector2 end)
    {
        Vector2 lengthSize = (end - start) * 0.5f;

        return new Vector3(start.x + lengthSize.x, 0.1f, start.y + lengthSize.y);
    }
    public Vector3 RopeScale(Vector2 start, Vector2 end)
    {
        Vector3 result;
        Vector2 delta = end - start;
        float deltaScale = Mathf.Abs(delta.x + delta.y);

        result = new Vector3(0.2f, 0.2f, deltaScale);

        return result;
    }
    public void ropedeta() // ������ ����. ���̶� ���� ����.
    {
        if(ropeStruct.ropePositions.Count<1)
        {
            ropeStruct.ropePositions.Add(transform.position); // Ÿ���� ��ġ.(������)
            ropeStruct.ropePositions.Add(transform.position);// �÷��̾��� ��ġ(������)
            NetworkObject ropeObject = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Rope));
            ropeObject.transform.position = RopePosition(ropeStruct.ropePositions[0], ropeStruct.ropePositions[1]);
            ropeObject.transform.localScale = RopeScale(ropeStruct.ropePositions[0], ropeStruct.ropePositions[1]);
            ropeStruct.ropeObjects.Add(ropeObject);
        }
        else
        {
            Vector2 delta = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 3] - ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1];
            if(delta.x == 0 || delta.y== 0)
            {
                // ������ ���� �÷��̾� ��ġ�� ����
            }
            else
            {
                // �÷��̾� ��ġ�� ropePosition�� ����.

            }
        }
    }


}
