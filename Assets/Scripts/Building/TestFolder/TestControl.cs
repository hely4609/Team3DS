using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestControl : MonoBehaviour
{
    //public Building building;
    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.UpArrow))
    //    {
    //        transform.position += new Vector3(0, 0, 1);
    //        building.TiledBuildingPos += new Vector2Int(0, 1);
    //        building.CheckBuild();

    //    }
    //    if (Input.GetKeyDown(KeyCode.DownArrow))
    //    {
    //        transform.position += new Vector3(0, 0, -1);
    //        building.TiledBuildingPos += new Vector2Int(0, -1);
    //        building.CheckBuild();
    //    }
    //    if (Input.GetKeyDown(KeyCode.RightArrow))
    //    {
    //        transform.position += new Vector3(1, 0, 0);
    //        building.TiledBuildingPos += new Vector2Int(1, 0);
    //        building.CheckBuild();
    //    }
    //    if (Input.GetKeyDown(KeyCode.LeftArrow))
    //    {
    //        transform.position += new Vector3(-1, 0, 0);
    //        building.TiledBuildingPos += new Vector2Int(-1, 0);
    //        building.CheckBuild();
    //    }
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        for (int i = 0; i < GameManager.Instance.BuildingManager.Buildings.Count; i++)
    //        {
    //            Debug.Log($"{GameManager.Instance.BuildingManager.Buildings[i].transform.position} / {GameManager.Instance.BuildingManager.Buildings.Count}");
    //        }
    //    }
    //}
    public EnergyBarrierGenerator gen;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            gen.TakeDamage(1);
        }
        if(Input.GetKeyDown(KeyCode.S)) 
        {
            gen.RepairBarrier();
        }
    }

}
