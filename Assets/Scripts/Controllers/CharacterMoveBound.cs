using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveBound : Manager
{
    public Vector3 BoundLeftUp { get; private set; }
    public Vector3 BoundRightDown { get; private set; }

//-57 30, 57 -57
//-57 60, 57 -57
//-90 60, 57 -57
//-90 60, 57 -90
//-90 60, 90 -90
//-90 90, 90 -90

    public override IEnumerator Initiate()
    {
        DrawBound(new Vector3(-57, 0, 30), new Vector3(57, 0, -57));
        yield return null;
    }

    public void DrawBound(Vector3 leftUp, Vector3 rightDown)
    {
        BoundLeftUp = leftUp;
        BoundRightDown = rightDown;
    }
}
