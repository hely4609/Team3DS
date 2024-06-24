using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Manager
{ 
    protected Dictionary<ResourceEnum.Prefab, Queue<GameObject>> poolDictionary;
    public IEnumerator ClaimPool() { return default; }
    public override IEnumerator Initiate() { return default; }
    protected void ReadStock() { }
    public GameObject Instantiate(ResourceEnum.Prefab target) { return default; }
    public GameObject Instantiate(ResourceEnum.Prefab target, Vector3 pos) { return default; }
    public GameObject Instantiate(ResourceEnum.Prefab target, Vector3 pos, Vector3 euler) { return default; }
    public GameObject Instantiate(ResourceEnum.Prefab target, Transform parent) { return default; }
    public void Destroy(GameObject target) { }
    public void Destroy(GameObject target, float time) { }
    public void Destroy(PoolingInfo info) { }
    public void Destroy(PoolingInfo info, float time) { }
}
