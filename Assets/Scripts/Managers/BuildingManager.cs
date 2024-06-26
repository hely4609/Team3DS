using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : Manager
{
    protected static List<Building> buildings = new List<Building>();
    public static List<Building> Buildings => buildings;

    public override IEnumerator Initiate() { yield return null; }
}
