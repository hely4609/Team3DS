using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingInfo : MyComponent
{
    private ResourceEnum.Prefab origin;
    public ResourceEnum.Prefab Origin => origin;
    private Queue<GameObject> originPool;
    [SerializeField]private float lifespan;
    public float Lisfespan 
    { 
        get => lifespan; 
        set
        {
            if(value == -1) lifespan = value;
            else if (value >= 0)
            {
                lifespan = Mathf.Min(value, lifespan);
            }
        }
    }

    protected override void MyUpdate(float deltaTime)
    {
        if (lifespan < 0 && lifespan > -1f)
        {
            GameManager.Instance.PoolManager.Destroy(this);
        }
        else lifespan -= deltaTime;
    }

    protected override void MyDestroy()
    {
        // 파괴된 후에 초기화
        lifespan = -1f;
    }

    public void SetInfo(ResourceEnum.Prefab wantOrigin, Queue<GameObject> wantQueue) 
    { 
        origin = wantOrigin;
        originPool = wantQueue;
    }
}
