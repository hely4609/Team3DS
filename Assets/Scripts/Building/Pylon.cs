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
    // 로프 관련 기능
    protected RopeStruct ropeStruct;

    protected override void Initialize()
    {
        // 디폴트 값.
        type = BuildingEnum.Pylon;
        buildingTimeMax = 10;
        size = new Vector2Int(1, 1);
        powerRange = 20;
    }
    public override Interaction InteractionStart(Player player)
    {
        // 완성이 아직 안됨.
        if (CompletePercent < 1)
        {
            return Interaction.Build;
        }
        else
        {
            // 줄을 집을거임.
            return Interaction.takeRope;
        }
    }
    // 줄 기능.
    public virtual void Rope(Player player)
    {
        // 실질적으로 이 기능은 플레이어가 실행할것.
        // 1. 플레이어가 움직이면 해당 위치에 전선이 깔림
        // 2. 전선이 깔리면 powerRange가 그만큼 줄어듬
        // 3. powerRange <= 0 가 되면 못움직임.
        // 4-1. 놓을 수 있는 공간이라고 하면 설치.(설치 할 때에도 CheckBuild할거라 ㄱㅊ)
        // 4-2. 취소키를 눌러서 전선도 없어지고 손에 든 빌딩도 없어짐.

        // 전선이 깔리는건 캐릭터가 실질적으로 이동할 때 만 깔리고,
        // 캐릭터와 타워까지의 거리에는 소비는 되지만 깔리지는 않음.
        float remainRope = powerRange;
        Vector3 pos = (transform.position - player.transform.position);
        remainRope -= Mathf.Sqrt(Mathf.Pow(pos.x, 2) + Mathf.Pow(pos.y, 2));
        
    }
    public void SetRope(Vector3Int beforePos, Vector3Int nowPos)
    {
        // 이전 위치 부터 현재 위치를 받아오고, 그 사이에 줄을 연결함.
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
    public void ropedeta() // 전선을 놓기. 길이랑 같은 원리.
    {
        if(ropeStruct.ropePositions.Count<1)
        {
            ropeStruct.ropePositions.Add(transform.position); // 타워의 위치.(정수값)
            ropeStruct.ropePositions.Add(transform.position);// 플레이어의 위치(정수값)
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
                // 마지막 값을 플레이어 위치로 수정
            }
            else
            {
                // 플레이어 위치를 ropePosition에 저장.

            }
        }
    }


}
