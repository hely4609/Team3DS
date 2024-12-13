using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public struct RopeStruct
{
    public List<NetworkObject> ropeObjects;
    public List<Vector3> ropePositions;
}
public class InteractableBuilding : Building, IInteraction
{
    [SerializeField] protected string objectName;
    [SerializeField] protected string localeName;
    public string ObjectName { get { return objectName; } }
    //protected Collider[] interactionColliders;
    //[SerializeField] protected Renderer interactionRenderer; // 상호작용 기준이될 Base Renderer 등록
    [Networked, SerializeField] protected bool IsSettingRope { get; set; } = false;
    [Networked, SerializeField] public bool IsRoped { get; set; } = false;
    [SerializeField] protected RopeStruct ropeStruct = new RopeStruct();
    public RopeStruct RopeStruct { get { return ropeStruct; } }
    [SerializeField] protected float maxRopeLength;
    [SerializeField] protected float currentRopeLength;

    [Networked] protected bool IsChangeInfo { get; set; }

    protected override void Initialize()
    {
        ropeStruct.ropeObjects = new List<NetworkObject>();
        ropeStruct.ropePositions = new List<Vector3>();
        ropeStruct.ropePositions.Add(transform.position);
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
    public virtual Interaction InteractionStart(Player player, Interaction interactionType)
    {
        //Player localPlayer = GameManager.Instance.NetworkManager.LocalController.ControlledPlayer;
        switch (interactionType)
        {
            case Interaction.Build:
                break;
            case Interaction.takeRope:
                break;
            case Interaction.Demolish:
                GameManager.Instance.BuildingManager.supply.TotalOreAmount += cost;
                Runner.Despawn(GetComponent<NetworkObject>());
                //localPlayer.RenewalInteractionUI(this, false);
                break;
        }

        return interactionType;
        
        
    }
    public virtual float InteractionUpdate(float deltaTime, Interaction interaction) // 상호작용시 적용할 함수. 제작하라는 명령이 들어오면 제작을 진행함.
    {
        if (interaction == Interaction.Build)
        {
            BuildBuilding(deltaTime);
        }
        return CompletePercent;
    }

    public virtual bool InteractionEnd(Player player, Interaction interactionType)
    {
        switch (interactionType)
        {
            case Interaction.Build:
            IsChangeInfo = !IsChangeInfo;
                break;
    }
        
        
        return true;
    }
    //public virtual string LocaleNameSet(string str)
    //{
    //    LocalizedString localizedString = new LocalizedString() { TableReference = "ChangeableTable", TableEntryReference = str };
    //    var stringOperation = localizedString.GetLocalizedStringAsync();
    //    if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        return stringOperation.Result;
    //    }
    //    return "고장남";
    //}
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

    public virtual List<Interaction> GetInteractions(Player player)
    {
        List<Interaction> currentAbleInteractions = new List<Interaction>();

        if (CompletePercent < 1)
        {
            currentAbleInteractions.Add(Interaction.Build);
            currentAbleInteractions.Add(Interaction.Demolish);
            return currentAbleInteractions;
        }
        else
        {
            currentAbleInteractions.Add(Interaction.takeRope);
            return currentAbleInteractions;
        }
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
    public virtual bool CheckRopeLength(Vector3 end, int number) // 전선을 끌수 있었나?
    {
        Vector3 start = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1];
        Vector3 delta = end - start;
        if (currentRopeLength > 0)
        {
            
            return true;
        }
        else return false;
    }
    public virtual void OnRopeSet(Vector3 playerPosition, int number) // 전선을 놓기. 길이랑 같은 원리.
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
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (ropeStruct.ropePositions.Count >= 3 && ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 3] == ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 1])
        {

            ropeStruct.ropePositions.RemoveRange(ropeStruct.ropePositions.Count - 2, 2);
            NetworkObject target = ropeStruct.ropeObjects[ropeStruct.ropeObjects.Count - 1];
            currentRopeLength += target.gameObject.transform.localScale.z;
            Runner.Despawn(target);
            ropeStruct.ropeObjects.Remove(target);

            foreach (var player in players)
            {
                var playerCS = player.GetComponent<Player>();
                if (number == playerCS.PossesionController.MyNumber)
                {
                    playerCS.angleCheckVector = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 2];
                }
            }

            return;
        }
        Vector3 start = ropeStruct.ropePositions[ropeStruct.ropePositions.Count - 2];


        foreach (var player in players)
        {
            var playerCS = player.GetComponent<Player>();
            if (number == playerCS.PossesionController.MyNumber)
            {
                playerCS.angleCheckVector = start;
            }
        }

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
