using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : Manager
{
    protected List<Building> buildings = new List<Building>();
    public List<Building> Buildings => buildings;

    public override IEnumerator Initiate() { yield return null; }
}
