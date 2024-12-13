using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PylonBuilding : Pylon
{
    protected override void Initialize()
    {
        base.Initialize();
        objectName = "Pylon";

        cost = 10;
    }
}
