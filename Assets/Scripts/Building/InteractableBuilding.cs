using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
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
    //[SerializeField] protected Renderer interactionRenderer; // 상호작용 기준이될 Base Renderer 등록
    [Networked, SerializeField]protected bool IsRoped { get; set; } = false;
    [SerializeField] protected RopeStruct ropeStruct = new RopeStruct();
    public RopeStruct RopeStruct { get { return ropeStruct; } }
    protected override void Initialize()
    {
        ropeStruct.ropeObjects = new List<NetworkObject>();
        ropeStruct.ropePositions = new List<Vector2>();
    }
    // 위치를 고정함.


    // 상호작용을 시작함.
    // 상호작용을 할 분류를 받아와서 어떤 걸 할 지 저장.
    // 플레이어가 

    // 스타트와 엔드는 단발성.
    // 플레이어는 상호작용 키 만 누를거임. 내용은 모름.
    // 뭘 할지는 얘가 플레이어한테 알려줄거임.
    // 플레이어는 건설, 수리이면 망치를 휘두를거고, onoff 면 손만 나와서 누를거고, 납품이면 손에 있는게 사라질거임.
    // 지속 = 업데이트.

    // 아무것도 없는 깡통건물. 세우기만 하고 상호작용 없음. ex) 육교
    public virtual Interaction InteractionStart(Player player)
    {
        // 완성이 아직 안됨.
        if (CompletePercent < 1)
        {
            return Interaction.Build;
        }
        else
        {
            return Interaction.takeRope;
        }
    }
    public virtual float InteractionUpdate(float deltaTime, Interaction interaction) // 상호작용시 적용할 함수. 제작하라는 명령이 들어오면 제작을 진행함.
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
        foreach(var rope in ropeStruct.ropeObjects)
        {
            Runner.Despawn(rope);
        }
        ropeStruct.ropeObjects.Clear();
        ropeStruct.ropePositions.Clear();
        ropeStruct.ropePositions.Add(startPos);
        IsRoped = false;
    }
    public void OnRopeSet(Vector2 playerPosition) // 전선을 놓기. 길이랑 같은 원리.
    {
        if (HasStateAuthority)
        {
            IsRoped = true;
            ropeStruct.ropePositions.Add(playerPosition);
            CreateRope();
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
        
        NetworkObject ropeObject = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Rope), new Vector3(start.x, 0, start.y), Quaternion.Euler(new Vector3(0, deltaAngle, 0)));
        ropeStruct.ropeObjects.Add(ropeObject);
        ropeObject.transform.localScale = new Vector3(1, 1, delta.magnitude);
        ropeObject.GetComponent<Rope>().Scale = delta.magnitude;
    }
}
