using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceManager : Manager
{
    public static int resourceAmount = 0;
    public static int resourceLoadCompleted = 0;
    public override IEnumerator Initiate()
    {
        yield return base.Initiate();
    }
}
