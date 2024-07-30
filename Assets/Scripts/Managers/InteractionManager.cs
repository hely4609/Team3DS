using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InteractionManager : Manager
{
    // List<IInteraction>  =  IInteraction이 생성되면 여기에 넣을겁니다.
    // IInteraction의 mesh

    // OnTrigger 부딪혔을때 1번 딱 실행되고 끝나는데
    // 이방법은 일정 주기마다. 갱신을해줘야됩니다.
    // 그래서 코드를 잘못쳣더니 
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
        //    mesh.bounds.SqrDistance(point) <= range // 인식범위안에 들어왔다.
        //}

        //새로추가됫다.
        //인식범위에서 나갔다.
    }
}
