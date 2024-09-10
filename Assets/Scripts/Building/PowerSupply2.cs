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
    }
}
