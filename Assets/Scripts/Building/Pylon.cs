using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pylon : InteractableBuilding
{
    protected int cost;
    // protected int powerCurrent;
    protected float powerRange;

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

}
