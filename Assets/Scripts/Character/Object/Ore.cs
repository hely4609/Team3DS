using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : MyComponent
{
    public int amount;

    public void Initialize()
    {
        amount = 1;
    }

    protected override void MyStart()
    {
        amount = 1;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.TryGetComponent(out Player player))
    //    {
    //        player.OreAmount += amount;
    //        Runner.Despawn(GetComponent<NetworkObject>());
    //    }

    //}
}
