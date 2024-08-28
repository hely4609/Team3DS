using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleanner : MyComponent
{
    [SerializeField] private Player owner;

    private List<Ore> targets = new();

    [Range(0, 1), SerializeField]
    private float suctionPower = 0.2f;

    protected override void MyStart()
    {
        if (owner != null) return;

        owner = GetComponentInParent<Player>();
    }

    protected override void MyUpdate(float deltaTime)
    {
        Suction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ore target))
        {
            targets.Add(target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Ore target))
        {
            targets.Remove(target);
        }
    }
    private void Suction()
    {
        if (targets.Count == 0) return;

        foreach (var target in targets)
        {
            if (target != null)
            {
                target.transform.position = Vector3.Lerp(target.transform.position, transform.position, suctionPower);
                if (Vector3.Distance(target.transform.position, transform.position) < 0.5f)
                {
                    owner.OreAmount += target.amount;
                    Debug.Log($"OreAmount = {owner.OreAmount}");
                    Runner.Despawn(target.GetComponent<NetworkObject>());
                }
            }
        }
    }
}
