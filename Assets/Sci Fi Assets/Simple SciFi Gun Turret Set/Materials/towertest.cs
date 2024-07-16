using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class towertest : Tower
{
    [Range(0,1)]
    [SerializeField] float value;
    [SerializeField] bool isBuildComplete;
    void Start()
    {
        HeightCheck();
    }

    protected void HeightCheck()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            i++;
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);

        float max = mesh.bounds.max.y;
        float min = mesh.bounds.min.y;
        Debug.Log(mesh.bounds.max.y);
        Debug.Log(mesh.bounds.min.y);

        Debug.Log(mesh.bounds.max.y + Mathf.Abs(mesh.bounds.min.y));

        foreach (var r in meshes)
        {
            r.material.SetFloat("_HeightMin", min);
            r.material.SetFloat("_HeightMax", max);
        }
    }
    // Update is called once per frame
    void Update()
    {
        foreach (var r in meshes) 
        {
            r.material.SetFloat("_CompletePercent", value);
        }

        if (isBuildComplete)
        {
            foreach(var r in meshes)
                r.material = ResourceManager.Get(ResourceEnum.Material.Turret1a);
        }
    }
}
