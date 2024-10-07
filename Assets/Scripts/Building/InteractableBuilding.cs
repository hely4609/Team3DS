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
    public List<Vector3> ropePositions;
}
public class InteractableBuilding : Building, IInteraction
{
    [SerializeField] protected string objectName;
    public string ObjectName { get { return objectName; } }
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
        ropeStruct.ropePositions = new List<Vector3>();
        ropeStruct.ropePositions.Add(transform.position);
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
        ropeStruct.ropePositions[0] = transform.position;
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
        ropeStruct.ropePositions.Add(transform.position);
        currentRopeLength = maxRopeLength;
        player.CanSetRope = true;
        IsSettingRope = false;
    }
    public virtual bool CheckRopeLength(Vector3 end, int number) // ������ ���� �־���?
    {
        Vector3 start = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1];
        Vector3 delta = end - start;
        if (currentRopeLength > 0)
        {
            
            return true;
        }
        else return false;
    }
    public virtual void OnRopeSet(Vector3 playerPosition, int number) // ������ ����. ���̶� ���� ����.
    {
        if (HasStateAuthority)
        {
            IsSettingRope = true;
            if (ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1].y > playerPosition.y)
            {
                if(playerPosition.y<4.7)
                {
                    playerPosition.y = (int)playerPosition.y-1;
                    if (playerPosition.y < 0)
                        playerPosition.y = 0;
                }
                else
                {
                    playerPosition.y = (int)Mathf.Round(playerPosition.y);
                }
            }
            else
            {
                playerPosition.y = (int)Mathf.Round(playerPosition.y);

            }

            ropeStruct.ropePositions.Add(playerPosition);
            CreateRope(number);
        }
    }
    public virtual void CreateRope(int number)
    {
        if (ropeStruct.ropePositions.Count >= 3 && ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 3] == ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1])
        {

            ropeStruct.ropePositions.RemoveRange(ropeStruct.ropePositions.Count - 2, 2);
            NetworkObject target = ropeStruct.ropeObjects[ropeStruct.ropeObjects.Count - 1];
            currentRopeLength += target.gameObject.transform.localScale.z;
            Runner.Despawn(target);
            ropeStruct.ropeObjects.Remove(target);

            return;
        }
        Vector3 start = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 2];
        Vector3 end = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1];
        Vector3 delta = end - start;

        currentRopeLength -= delta.magnitude;
        
        NetworkObject ropeObject = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Rope), new Vector3(start.x, start.y, start.z), Quaternion.LookRotation(delta));
        //ropeObject.transform.LookAt(end);

        ropeStruct.ropeObjects.Add(ropeObject);
        ropeObject.transform.localScale = new Vector3(1, 1, delta.magnitude);
        ropeObject.GetComponent<Rope>().Scale = delta.magnitude;
    }
    virtual public void AttachRope(Player player, int number)
    {
    }
}
