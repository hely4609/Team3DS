using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : NetworkBehaviour
{
    [Networked] public float Scale { get; set; }
    public override void Spawned()
    {
        transform.localScale = new Vector3(1,1,Scale);
    }
}
