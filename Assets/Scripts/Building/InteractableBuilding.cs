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
    //[SerializeField] protected Renderer interactionRenderer; // 상호작용 기준이될 Base Renderer 등록
    protected bool isRoped = false;
    protected RopeStruct ropeStruct = new RopeStruct();
    protected override void Initialize()
    {
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
            return Interaction.None;
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


    public void ResetRope()
    {
        ropeStruct.ropeObjects.Clear();
        ropeStruct.ropePositions.Clear();
    }
    public void SetRope(InteractableBuilding building) // 건물을 터치하면 그 건물이랑 전선 연결함.
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
    public void OnRopeSet(Vector2 playerPosition) // 전선을 놓기. 길이랑 같은 원리.
    {
        if (ropeStruct.ropePositions.Count < 1)
        {
            ropeStruct.ropePositions.Add(transform.position); // 타워의 위치.(정수값)
            ropeStruct.ropePositions.Add(playerPosition);// 플레이어의 위치(정수값)
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
                // 마지막 값을 플레이어 위치로 수정
                ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1] = playerPosition;
            }
            else
            {
                // 플레이어 위치를 ropePosition에 저장.
                ropeStruct.ropePositions.Add(playerPosition);
                NetworkObject ropeObject = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Rope));
                ropeStruct.ropeObjects.Add(ropeObject);
            }
            ropeStruct.ropeObjects[ropeStruct.ropeObjects.Count - 1].transform.position = RopePosition(ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 2], ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1]);
            ropeStruct.ropeObjects[ropeStruct.ropeObjects.Count - 1].transform.localScale = RopeScale(ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 2], ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1]);
        }
    }


}
