using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
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
    [SerializeField] protected RopeStruct ropeStruct = new RopeStruct();
    public RopeStruct RopeStruct { get { return ropeStruct; } }
    protected override void Initialize()
    {
        ropeStruct.ropeObjects = new List<NetworkObject>();
        ropeStruct.ropePositions = new List<Vector2>();
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
            return Interaction.takeRope;
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

    public override bool FixPlace()
    {
        bool toReturn = base.FixPlace();
        ropeStruct.ropePositions.Add(startPos);
        return toReturn;
    }


    public void ResetRope()
    {
        ropeStruct.ropeObjects.Clear();
        ropeStruct.ropePositions.Clear();
        ropeStruct.ropePositions.Add(startPos);
    }
    public void SetRope(InteractableBuilding building) // �ǹ��� ��ġ�ϸ� �� �ǹ��̶� ���� ������.
    {
        if (!building.isRoped)
        {
            building.isRoped = true;
            GameManager.Instance.BuildingManager.RopeStructs.Add(ropeStruct);
            ResetRope();
        }
    }
    public void OnRopeSet(Vector2 playerPosition) // ������ ����. ���̶� ���� ����.
    {
        if (HasStateAuthority)
        {
            Vector2 delta;
            ropeStruct.ropePositions.Add(playerPosition);
            delta = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 2] - ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1];
            CreateRope();
            //    Vector2 delta;
            //    if (ropeStruct.ropePositions.Count < 1)
            //    {
            //        delta = playerPosition - StartPos;
            //        ropeStruct.ropePositions.Add(StartPos); // Ÿ���� ��ġ.(������)
            //        if (delta.x == 0 || delta.y == 0)
            //        {
            //            ropeStruct.ropePositions.Add(playerPosition);// �÷��̾��� ��ġ(������)
            //            CreateRope();
            //        }
            //        else
            //        {
            //            ropeStruct.ropePositions.Add(new Vector2(StartPos.x, playerPosition.y));
            //            CreateRope();

            //            ropeStruct.ropePositions.Add(playerPosition);
            //            CreateRope();
            //            ropeStruct.ropeObjects[RopeStruct.ropeObjects.Count - 1].transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            //        }
            //        Debug.Log($"{ropeStruct.ropePositions.Count} ����Ʈ ����");
            //    }
            //    else
            //    {
            //        if (RopeStruct.ropePositions[RopeStruct.ropePositions.Count - 1] != playerPosition)
            //        {
            //            ropeStruct.ropePositions.Add(playerPosition);
            //            CreateRope();
            //            delta = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 2] - ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1];
            //            if (delta.x != 0)
            //            {
            //                ropeStruct.ropeObjects[ropeStruct.ropeObjects.Count - 1].transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            //            }
            //        }
            //    }
        }
    }
    public void CreateRope()
    {
        Vector2 start = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 2];
        Vector2 end = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1];
        Vector2 delta = end - start;

        float deltaAngle = Vector2.Angle(delta, Vector2.up);
        int deltaAngleCorrection = 1;
        if (delta.x < 0) deltaAngleCorrection = -1;
        deltaAngle = Vector2.Angle(delta, Vector2.up) * deltaAngleCorrection;
        Debug.Log($"start : {start}, delta : {delta}, deltaAngle : {deltaAngle}");

        NetworkObject ropeObject = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Rope), new Vector3(start.x, 0, start.y), Quaternion.Euler(new Vector3(0, deltaAngle, 0)));
        ropeStruct.ropeObjects.Add(ropeObject);
    }


}
