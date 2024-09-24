using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class PowerSupply2 : Pylon
{
    protected override void Initialize()
    {
        ropeStruct.ropePositions.Add(transform.position);

        buildingType = BuildingEnum.Pylon;
        maxRopeLength = 30;
        currentRopeLength = maxRopeLength;
        
        multiTabList = new List<RopeStruct>();
        isSettingRopeList = new List<bool>();
        OnOff = true;
        for (int i = 0; i < 4; i++)
        {
            RopeStruct ropes = new RopeStruct();
            ropes.ropePositions = new List<Vector3>();
            ropes.ropeObjects = new List<NetworkObject>();
            multiTabList.Add(ropes);

            multiTabList[i].ropePositions.Add(transform.position);
            isSettingRopeList.Add(false);
            ropeLengthList.Add(maxRopeLength);
        }
    }
}
