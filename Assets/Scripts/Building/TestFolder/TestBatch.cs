using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBatch : MonoBehaviour
{
    private void OnEnable()
    {
        Building thisObject = GetComponent<Building>();
        BuildingManager.Buildings.Add(thisObject);
    }
}
