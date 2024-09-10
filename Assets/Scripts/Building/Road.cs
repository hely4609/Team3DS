using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Road : Building
{
    
    protected override void Initialize()
    {
        buildingType = BuildingEnum.Barrier;
    }
            
    public Vector2Int Size { get { return size; } set { 
            size = value;
            startPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
            GameManager.Instance.BuildingManager.AddBuilding(this); } }
}
