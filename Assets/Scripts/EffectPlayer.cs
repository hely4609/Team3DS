using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : NetworkBehaviour
{
    [SerializeField] Transform wantTF;
    public void PlayEffect(string wantEffect)
    {
        if(Enum.TryParse(wantEffect, out ResourceEnum.Prefab wantPrefab))
        {
            NetworkObject effect = Runner.Spawn(ResourceManager.Get(wantPrefab), wantTF.position);

            StartCoroutine(DespawnEffect(effect));
        }
    }

    IEnumerator DespawnEffect(NetworkObject effect)
    {
        yield return new WaitForSeconds(2);
        Runner.Despawn(effect);
    }
}
