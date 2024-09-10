using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Pylon : InteractableBuilding
{
    protected int cost;

    protected override void Initialize()
    {
        // 디폴트 값.
        buildingType = BuildingEnum.Pylon;
        buildingTimeMax = 1;
        size = new Vector2Int(1, 1);
        maxRopeLength = 20;
        currentRopeLength= maxRopeLength;
    }
    public override Interaction InteractionStart(Player player)
    {
        // 완성이 아직 안됨.
        if (CompletePercent < 1)
        {
            return Interaction.Build;
        }
        else if(player.ropeBuilding == null)
        {
            Vector2 playerTransformVector2 = new Vector2((int)(player.transform.position.x), (int)(player.transform.position.z));
            if (!IsSettingRope && player.ropeBuilding == null)
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
        if(building is Tower)
        //if (building.GetType().IsSubclassOf(typeof(Tower)))
        {
            Vector2 thisVector2 = new Vector2((int)(transform.position.x), (int)(transform.position.z));
            
            building.OnRopeSet(thisVector2);
            IsSettingRope = false;
            building.IsRoped = true;
        }
    }
}
