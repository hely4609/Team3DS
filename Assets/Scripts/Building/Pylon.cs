using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class Pylon : InteractableBuilding
{
    protected int cost;
    protected List<RopeStruct> multiTabList;
    public List<RopeStruct> MultiTabList { get { return multiTabList; } }
    public List<bool> isSettingRopeList;
    protected override void Initialize()
    {
        GameManager.Instance.BuildingManager.PylonList.Add(this);
        // 디폴트 값.
        buildingType = BuildingEnum.Pylon;
        buildingTimeMax = 1;
        size = new Vector2Int(1, 1);
        maxRopeLength = 20;
        currentRopeLength = maxRopeLength;
        multiTabList = new List<RopeStruct>();
        isSettingRopeList = new List<bool>();
        for (int i = 0; i < 4; i++)
        {
            RopeStruct ropes = new RopeStruct();
            ropes.ropePositions = new List<Vector2>();
            ropes.ropeObjects = new List<NetworkObject>();
            multiTabList.Add(ropes);

            multiTabList[i].ropePositions.Add(startPos);
            isSettingRopeList.Add(false);
        }

    }
    public override Interaction InteractionStart(Player player)
    {
        // 완성이 아직 안됨.
        if (CompletePercent < 1)
        {
            return Interaction.Build;
        }
        else if (player.ropeBuilding == null)
        {
            int myNumber = GameManager.Instance.NetworkManager.LocalController.myAuthority.PlayerId;

            Vector2 playerTransformVector2 = new Vector2((int)(player.transform.position.x), (int)(player.transform.position.z));
            if (!isSettingRopeList[myNumber] && player.ropeBuilding == null)
            {
                OnRopeSet(playerTransformVector2);
                player.ropeBuilding = this;
                // 줄을 집을거임.
                return Interaction.takeRope;
            }
        }
        else
        {
            AttachRope(player.ropeBuilding);
            player.ropeBuilding = null;
        }
        return Interaction.None;
    }
    public override void AttachRope(InteractableBuilding building)
    {
        int myNumber = GameManager.Instance.NetworkManager.LocalController.myAuthority.PlayerId;

        if (building is Tower)
        //if (building.GetType().IsSubclassOf(typeof(Tower)))
        {
            Vector2 thisVector2 = new Vector2((int)(transform.position.x), (int)(transform.position.z));

            building.OnRopeSet(thisVector2);
            isSettingRopeList[myNumber] = false;
            building.IsRoped = true;
        }
    }

    public override void ResetRope(Player player)
    {
        int myNumber = GameManager.Instance.NetworkManager.LocalController.myAuthority.PlayerId;

        foreach (var rope in multiTabList[myNumber].ropeObjects)
        {
            Runner.Despawn(rope);
        }
        multiTabList[myNumber].ropeObjects.Clear();
        multiTabList[myNumber].ropePositions.Clear();
        multiTabList[myNumber].ropePositions.Add(startPos);
        currentRopeLength = maxRopeLength;
        player.CanSetRope = true;
        isSettingRopeList[myNumber] = false;
    }
    public override bool CheckRopeLength(Vector2 end) // 전선을 끌수 있었나?
    {
        int myNumber = GameManager.Instance.NetworkManager.LocalController.myAuthority.PlayerId;

        Vector2 start = multiTabList[myNumber].ropePositions[multiTabList[myNumber].ropePositions.Count - 1];
        Vector2 delta = end - start;
        if (currentRopeLength > 0)
        {
            currentRopeLength -= delta.magnitude;
            return true;
        }
        else return false;
    }
    public override void OnRopeSet(Vector2 playerPosition) // 전선을 놓기. 길이랑 같은 원리.
    {
        int myNumber = GameManager.Instance.NetworkManager.LocalController.myAuthority.PlayerId;
        if (HasStateAuthority)
        {
            IsSettingRope = true;
            multiTabList[myNumber].ropePositions.Add(playerPosition);
            Debug.Log($"{multiTabList[myNumber].ropePositions[multiTabList[myNumber].ropePositions.Count - 1]}");

            CreateRope();
        }
    }
    public override void CreateRope()
    {
        int myNumber = GameManager.Instance.NetworkManager.LocalController.myAuthority.PlayerId;

        Vector2 start = multiTabList[myNumber].ropePositions[multiTabList[myNumber].ropePositions.Count - 2];
        Vector2 end = multiTabList[myNumber].ropePositions[multiTabList[myNumber].ropePositions.Count - 1];
        Vector2 delta = end - start;

        float deltaAngle = Vector2.Angle(delta, Vector2.up);
        int deltaAngleCorrection = 1;
        if (delta.x < 0) deltaAngleCorrection = -1;
        deltaAngle = Vector2.Angle(delta, Vector2.up) * deltaAngleCorrection;

        NetworkObject ropeObject = GameManager.Instance.NetworkManager.Runner.Spawn(ResourceManager.Get(ResourceEnum.Prefab.Rope), new Vector3(start.x, 0, start.y), Quaternion.Euler(new Vector3(0, deltaAngle, 0)));
        multiTabList[myNumber].ropeObjects.Add(ropeObject);
        ropeObject.transform.localScale = new Vector3(1, 1, delta.magnitude);
        ropeObject.GetComponent<Rope>().Scale = delta.magnitude;
    }
}

