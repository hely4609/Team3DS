using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : MyComponent
{
    public int amount;

    protected Player target; 

    public void Initialize()
    {
        amount = 1;
    }

    protected override void MyStart()
    {
        amount = 1;
    }

    protected override void MyUpdate(float deltaTime)
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position,0.2f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.OreAmount += amount;
            Runner.Despawn(GetComponent<NetworkObject>());
        }
    }

    public void SetTarget(Player player)
    {
        if (target != null && player != null) return;  
        
        target = player;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        SoundManager.Play(ResourceEnum.SFX.coin, transform.position);
    }
    
}
