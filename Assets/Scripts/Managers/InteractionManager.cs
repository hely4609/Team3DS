using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InteractionManager : Manager
{
    // List<IInteraction>  =  IInteraction�� �����Ǹ� ���⿡ �����̴ϴ�.
    // IInteraction�� mesh

    // OnTrigger �ε������� 1�� �� ����ǰ� �����µ�
    // �̹���� ���� �ֱ⸶��. ����������ߵ˴ϴ�.
    // �׷��� �ڵ带 �߸������� 
    void a()
    {
        Mesh mesh;
        //mesh.bounds.SqrDistance()
    }

    private void CheckInteractable(Vector3 point, float range)
    {
        //List<IInteraction> list = new List<IInteraction>();

        //foreach (var interaction in list)
        //{
        //    Mesh mesh = interaction.GetMesh();
        //    mesh.bounds.SqrDistance(point) <= range // �νĹ����ȿ� ���Դ�.
        //}

        //�����߰��̴�.
        //�νĹ������� ������.
    }
}
