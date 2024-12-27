using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveBound : Manager
{
    public Vector3 BoundLeftUp { get; private set; }
    public Vector3 BoundRightDown { get; private set; }

    public Vector3[] firstArea { get; private set; } = new Vector3[2] { new Vector3(-57, 0, 30), new Vector3(57, 0, -57) };
    public Vector3[] secondArea { get; private set; } = new Vector3[2] { new Vector3(-57, 0, 60), new Vector3(57, 0, -57) };
    public Vector3[] thirdArea { get; private set; } = new Vector3[2] { new Vector3(-90, 0, 60), new Vector3(57, 0, -57) };
    public Vector3[] fourthArea { get; private set; } = new Vector3[2] { new Vector3(-90, 0, 60), new Vector3(57, 0, -90) };
    public Vector3[] fifthArea { get; private set; } = new Vector3[2] { new Vector3(-90, 0, 60), new Vector3(90, 0, -90) };
    public Vector3[] sixthArea { get; private set; } = new Vector3[2] { new Vector3(-90, 0, 90), new Vector3(90, 0, -90) };

    public override IEnumerator Initiate()
    {
        DrawBound(firstArea[0], firstArea[1]);
        yield return null;
    }

    public void DrawBound(Vector3 leftUp, Vector3 rightDown)
    {
        BoundLeftUp = leftUp;
        BoundRightDown = rightDown;
    }
}
