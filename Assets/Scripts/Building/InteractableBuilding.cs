using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    [Networked, SerializeField] protected bool IsSettingRope { get; set; } = false;
    [Networked, SerializeField] public bool IsRoped { get; set; } = false;
    [SerializeField] protected RopeStruct ropeStruct = new RopeStruct();
    public RopeStruct RopeStruct { get { return ropeStruct; } }
    [SerializeField] protected float maxRopeLength;
    [SerializeField] protected float currentRopeLength;
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


    public virtual void ResetRope(Player player, int number)
    {
        foreach (var rope in ropeStruct.ropeObjects)
        {
            Runner.Despawn(rope);
        }
        ropeStruct.ropeObjects.Clear();
        ropeStruct.ropePositions.Clear();
        ropeStruct.ropePositions.Add(startPos);
        currentRopeLength = maxRopeLength;
        player.CanSetRope = true;
        IsSettingRope = false;
    }
    public virtual bool CheckRopeLength(Vector2 end, int number) // ������ ���� �־���?
    {
        Vector2 start = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1];
        Vector2 delta = end - start;
        if (currentRopeLength > 0)
        {
            currentRopeLength -= delta.magnitude;
            return true;
        }
        else return false;
    }
    public virtual void OnRopeSet(Vector2 playerPosition, int number) // ������ ����. ���̶� ���� ����.
    {
        if (HasStateAuthority)
        {
            IsSettingRope = true;
            ropeStruct.ropePositions.Add(playerPosition);
            Debug.Log($"{ropeStruct.ropePositions[RopeStruct.ropePositions.Count-1]}");
            
            CreateRope(number);
        }
    }
    public virtual void CreateRope(int number)
    {
        Vector2 start = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 2];
        Vector2 end = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1];
        Vector2 delta = end - start;

        float deltaAngle = Vector2.Angle(delta, Vector2.up);
        int deltaAngleCorrection = 1;
        if (delta.x < 0) deltaAngleCorrection = -1;
        deltaAngle = Vector2.Angle(delta, Vector2.up) * deltaAngleCorrection;

        NetworkObject ropeObject = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Rope), new Vector3(start.x, 0, start.y), Quaternion.Euler(new Vector3(0, deltaAngle, 0)));
        ropeStruct.ropeObjects.Add(ropeObject);
        ropeObject.transform.localScale = new Vector3(1, 1, delta.magnitude);
        ropeObject.GetComponent<Rope>().Scale = delta.magnitude;
    }
    virtual public void AttachRope(InteractableBuilding building, int number)
    {
    }
}
