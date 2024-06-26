using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBatch : MyComponent
{
    protected override void MyStart()
    {
        Building thisObject = GetComponent<Building>();
        GameManager.Instance.BuildingManager.Buildings.Add(thisObject);
    }
}
