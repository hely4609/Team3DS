using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public struct RopeStruct
{
    public List<NetworkObject> ropeObjects;
    public List<Vector2> ropePositions;
}
public class InteractableBuilding : Building, IInteraction
{
    [SerializeField] protected string objectName;
    //protected Collider[] interactionColliders;
    //[SerializeField] protected Renderer interactionRenderer; // ��ȣ�ۿ� �����̵� Base Renderer ���
    protected bool isRoped = false;
    protected RopeStruct ropeStruct = new RopeStruct();
    protected override void Initialize()
    {
    }
    // ��ġ�� ������.


    // ��ȣ�ۿ��� ������.
    // ��ȣ�ۿ��� �� �з��� �޾ƿͼ� � �� �� �� ����.
    // �÷��̾ 

    // ��ŸƮ�� ����� �ܹ߼�.
    // �÷��̾�� ��ȣ�ۿ� Ű �� ��������. ������ ��.
    // �� ������ �갡 �÷��̾����� �˷��ٰ���.
    // �÷��̾�� �Ǽ�, �����̸� ��ġ�� �ֵθ��Ű�, onoff �� �ո� ���ͼ� �����Ű�, ��ǰ�̸� �տ� �ִ°� ���������.
    // ���� = ������Ʈ.

    // �ƹ��͵� ���� ����ǹ�. ����⸸ �ϰ� ��ȣ�ۿ� ����. ex) ����
    public virtual Interaction InteractionStart(Player player)
    {
        // �ϼ��� ���� �ȵ�.
        if (CompletePercent < 1)
        {
            return Interaction.Build;
        }
        else
        {
            return Interaction.None;
        }
    }
    public virtual float InteractionUpdate(float deltaTime, Interaction interaction) // ��ȣ�ۿ�� ������ �Լ�. �����϶�� ����� ������ ������ ������.
    {
        if (interaction == Interaction.Build)
        {
            BuildBuilding(deltaTime);
            
        }
        return CompletePercent;
    }

    public virtual bool InteractionEnd()
    { 
        return false;
    }

    public Collider[] GetInteractionColliders()
    {
        return cols;
    }

    public Bounds GetInteractionBounds()
    {
        return default;
    }

    public virtual string GetName()
    {
        return objectName;
    }


    public void ResetRope()
    {
        ropeStruct.ropeObjects.Clear();
        ropeStruct.ropePositions.Clear();
    }
    public void SetRope(InteractableBuilding building) // �ǹ��� ��ġ�ϸ� �� �ǹ��̶� ���� ������.
    { 
        if(!building.isRoped)
        {
            building.isRoped= true;
            GameManager.Instance.BuildingManager.RopeStructs.Add(ropeStruct);
            ResetRope();
        }
    }
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
    public void OnRopeSet(Vector2 playerPosition) // ������ ����. ���̶� ���� ����.
    {
        if (ropeStruct.ropePositions.Count < 1)
        {
            ropeStruct.ropePositions.Add(transform.position); // Ÿ���� ��ġ.(������)
            ropeStruct.ropePositions.Add(playerPosition);// �÷��̾��� ��ġ(������)
            NetworkObject ropeObject = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Rope));
            ropeObject.transform.position = RopePosition(ropeStruct.ropePositions[0], ropeStruct.ropePositions[1]);
            ropeObject.transform.localScale = RopeScale(ropeStruct.ropePositions[0], ropeStruct.ropePositions[1]);
            ropeStruct.ropeObjects.Add(ropeObject);
        }
        else
        {
            Vector2 delta = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 3] - ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1];
            if (delta.x == 0 || delta.y == 0)
            {
                // ������ ���� �÷��̾� ��ġ�� ����
                ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1] = playerPosition;
            }
            else
            {
                // �÷��̾� ��ġ�� ropePosition�� ����.
                ropeStruct.ropePositions.Add(playerPosition);
                NetworkObject ropeObject = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Rope));
                ropeStruct.ropeObjects.Add(ropeObject);
            }
            ropeStruct.ropeObjects[ropeStruct.ropeObjects.Count - 1].transform.position = RopePosition(ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 2], ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1]);
            ropeStruct.ropeObjects[ropeStruct.ropeObjects.Count - 1].transform.localScale = RopeScale(ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 2], ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1]);
        }
    }


}
