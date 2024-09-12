using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class PowerSupply2 : Pylon
{
    protected override void Initialize()
    {
        buildingType = BuildingEnum.Pylon;
        maxRopeLength = 30;
        currentRopeLength = maxRopeLength;
        ropeStruct.ropePositions.Add(startPos);

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
            ropeLengthList.Add(maxRopeLength);
        }
    }
}
