using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PoolManager : Manager
{ 
    protected Dictionary<ResourceEnum.Prefab, Queue<GameObject>> poolDictionary = new();
    
    Transform poolContainer;
    public override IEnumerator Initiate() 
    {
        if (poolContainer == null) poolContainer = new GameObject("Pool Container").transform;
        yield return null; 
    }

    //                                              � ������, �� ��
    public IEnumerator ClaimPool(Dictionary<ResourceEnum.Prefab, int> dictionary, int numbersOnAFrame = 7) 
    {
        if(numbersOnAFrame < 1) numbersOnAFrame = 1;
        int count = 0;
        foreach(var keyValue in dictionary)
        {
            for(int i=0; i<keyValue.Value; i++)
            {
                ReadyStock(keyValue.Key);
                GameManager.ClaimLoadInfo($"{keyValue.Key} : {count} / {keyValue.Value}");
                count++;
                if(count % numbersOnAFrame == 0)
                {
                    yield return null;
                }
            }
        }
        yield return null; 
    }

    protected void ReadyStock(ResourceEnum.Prefab target) 
    { 
        // ��ųʸ��� Ű�� ������
        if(poolDictionary.TryGetValue(target, out Queue<GameObject> result))
        {
            poolDictionary[target] = result;
            
        }
        else
        {
            // ������ ť�� �����ؼ� Ű�� �ֱ�
            Queue<GameObject> queue = new Queue<GameObject>();
            poolDictionary.Add(target, queue);
        }

        GameObject inst = GameObject.Instantiate(ResourceManager.Get(target));
        inst.AddComponent<PoolingInfo>().SetInfo(target, result);
        inst.SetActive(false);
        poolDictionary[target].Enqueue(inst);
        //Debug.Log(inst.gameObject);
    }
    public GameObject Instantiate(ResourceEnum.Prefab target) 
    { 

        if (poolDictionary.TryGetValue(target, out Queue<GameObject> result))
        {
            if(result.Count == 0)
            {
                ReadyStock(target);
                poolDictionary.TryGetValue(target, out result);
            }
        }
        else
        {
            // ������ ��������
            Queue<GameObject> queue = new();
            poolDictionary.Add(target, queue);
            ReadyStock(target);
            poolDictionary.TryGetValue(target, out result);
        }
        GameObject inst = result.Dequeue();
        inst.SetActive(true);
        return inst; 
    }
    public GameObject Instantiate(ResourceEnum.Prefab target, Vector3 pos) 
    { 
        GameObject inst = Instantiate(target);
        inst.transform.position = pos;
        return inst; 
    }
    public GameObject Instantiate(ResourceEnum.Prefab target, Vector3 pos, Vector3 euler) 
    { 
        GameObject inst = Instantiate(target, pos);
        inst.transform.eulerAngles = euler;
        return inst; 
    }
    public GameObject Instantiate(ResourceEnum.Prefab target, Transform parent) 
    { 
        GameObject inst = Instantiate(target);
        // ��Ȱ��� Ǯ�� �������� Ʈ������ ���� �ٲ���� �� �����Ƿ� ���� �������� ������ �θ� �־��ش�
        GameObject origin = ResourceManager.Get(target);
        inst.transform.SetParent(parent);
        //inst.transform.localPosition = origin.transform.position;
        //inst.transform.localRotation = origin.transform.rotation;
        inst.transform.SetLocalPositionAndRotation(origin.transform.position, origin.transform.rotation);
        inst.transform.localScale = origin.transform.localScale; 

        return inst; 
    }
    public void Destroy(GameObject target) 
    { 
        if(target != null && target.TryGetComponent<PoolingInfo>(out PoolingInfo pool))
        {
            if(GameManager.Instance.PoolManager.poolDictionary.TryGetValue(pool.Origin, out Queue<GameObject> result))
            {
                result.Enqueue(target);
                target.SetActive(false);
            }
            else
            {
                GameObject.Destroy(target);
            }
        }
        else
        {
            GameObject.Destroy(target);
        }
    }
    public void Destroy(GameObject target, float time = 5f) 
    { 
        if(target.TryGetComponent<PoolingInfo>(out PoolingInfo pool))
        {
            pool.Lisfespan = time;
        }
        else
        {
            GameObject.Destroy(target, time);
        }
    }
    public void Destroy(PoolingInfo info)
    {
        if (GameManager.Instance.PoolManager.poolDictionary.TryGetValue(info.Origin, out Queue<GameObject> result))
        {
            result.Enqueue(info.gameObject);
            info.gameObject.SetActive(false);
        }

    }
}
