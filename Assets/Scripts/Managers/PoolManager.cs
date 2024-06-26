using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Manager
{ 
    protected Dictionary<ResourceEnum.Prefab, Queue<GameObject>> poolDictionary;
    
    Transform poolContainer;
    public override IEnumerator Initiate() 
    {
        if (poolContainer == null) poolContainer = new GameObject("Pool Container").transform;
        yield return null; 
    }

    //                                              어떤 프리팹, 몇 개
    public IEnumerator ClaimPool(Dictionary<ResourceEnum.Prefab, int> dictionary, int numbersOnAFrame = 7) 
    { 
        for(int i = 0;  i < poolDictionary.Count; i++)
        {

        }
        yield return null; 
    }

    protected void ReadStock(ResourceEnum.Prefab target) 
    { 
        // 딕셔너리에 키가 있으면
        if(poolDictionary.TryGetValue(target, out Queue<GameObject> result))
        {

        }
        else
        {
            // 없으면 큐를 생성해서 키에 넣기
            Queue<GameObject> queue = new Queue<GameObject>();
            poolDictionary.Add(target, queue);
        }
    }
    public GameObject Instantiate(ResourceEnum.Prefab target) { return default; }
    public GameObject Instantiate(ResourceEnum.Prefab target, Vector3 pos) { return default; }
    public GameObject Instantiate(ResourceEnum.Prefab target, Vector3 pos, Vector3 euler) { return default; }
    public GameObject Instantiate(ResourceEnum.Prefab target, Transform parent) { return default; }
    public void Destroy(GameObject target) { }
    public void Destroy(GameObject target, float time) { }
    public void Destroy(PoolingInfo info) { }
    public void Destroy(PoolingInfo info, float time) { }
}
