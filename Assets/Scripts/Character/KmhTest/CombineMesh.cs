using UnityEngine;
using System.Collections;

// Copy meshes from children into the parent's Mesh.
// CombineInstance stores the list of meshes.  These are combined
// and assigned to the attachedPylonList Mesh.

public class CombineMesh : MonoBehaviour
{
    void Start()
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
        
        Debug.Log(mesh.bounds.size.y);
        Debug.Log(mesh.bounds.max.y);
        Debug.Log(mesh.bounds.min.y);

        Debug.Log(mesh.bounds.max.y + Mathf.Abs(mesh.bounds.min.y));   
    }
}