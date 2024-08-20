using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractableBuilding : Building, IInteraction
{
    //protected Collider[] interactionColliders;
    //[SerializeField] protected Renderer interactionRenderer; // ��ȣ�ۿ� �����̵� Base Renderer ���

    protected override void Initialize()
    {
        //GameManager.Instance.InteractionManager.AddInteractionObject(this);
        //interactionRenderer = meshes[0];
    }
    // ��ġ�� ������.


    // ��ȣ�ۿ��� ������.
    // ��ȣ�ۿ��� �� �з��� �޾ƿͼ� � �� �� �� ����.
    // �÷��̾ 

    // ��ŸƮ�� ����� �ܹ߼�.
    // �÷��̾�� ��ȣ�ۿ� Ű �� ��������. ������ ��.
    // �� ������ �갡 �÷��̾����� �˷��ٰ���.
    // �÷��̾�� �Ǽ�, �����̸� ��ġ�� �ֵθ��Ű�, onoff �� �ո� ���ͼ� �����Ű�, ��ǰ�̸� �տ� �ִ°� ���������.
    // ���� = ������Ʈ.

    // �ƹ��͵� ���� ����ǹ�. ����⸸ �ϰ� ��ȣ�ۿ� ����. ex) ����
    public virtual Interaction InteractionStart(Player player)
    {
        // �ϼ��� ���� �ȵ�.
        if (CompletePercent < 1)
        {
            return Interaction.Build;
        }
        else
        {
            return Interaction.None;
        }
    }
    public virtual float InteractionUpdate(float deltaTime, Interaction interaction) // ��ȣ�ۿ�� ������ �Լ�. �����϶�� ����� ������ ������ ������.
    {
        if (interaction == Interaction.Build)
        {
            BuildBuilding(deltaTime);
        }
        return CompletePercent;
    }

    public bool InteractionEnd()
    { 
        return true;
    }

    public Collider[] GetInteractionColliders()
    {
        return cols;
    }

    public Bounds GetInteractionBounds()
    {
        return default;
    }

    public virtual string GetName()
    {
        return "InteractableBuilding";
    }



    //    protected override void HeightCheck()
    //    {
    //        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
    //        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

    //        int i = 0;
    //        while (i < meshFilters.Length)
    //        {
    //            combine[i].mesh = meshFilters[i].sharedMesh;
    //            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

    //            i++;
    //        }

    //        MeshFilter inst = gameObject.AddComponent<MeshFilter>();

    //        Mesh mesh = inst.mesh;
    //        mesh.Clear();
    //        mesh.CombineMeshes(combine);

    //        MeshCollider col = gameObject.AddComponent<MeshCollider>();
    //        col.sharedMesh = mesh;

    //#if UNITY_EDITOR
    //        { // Mesh ����
    //            string path = "Assets/MyMesh.asset";
    //            AssetDatabase.CreateAsset(transform.GetComponent<MeshFilter>().mesh, AssetDatabase.GenerateUniqueAssetPath(path));
    //            AssetDatabase.SaveAssets();
    //        }
    //#endif

    //        float max = mesh.bounds.max.y;
    //        float min = mesh.bounds.min.y;

    //        foreach (MeshRenderer r in meshes)
    //        {
    //            r.material.SetFloat("_HeightMin", min);
    //            Debug.Log(min);
    //            r.material.SetFloat("_HeightMax", max);
    //            Debug.Log(max);
    //        }
    //    }

}
