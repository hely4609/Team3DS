using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingInfo : MyComponent
{
    private ResourceEnum.Prefab origin;
    private Queue<GameObject> originPool;
    public ResourceEnum.Prefab Origin => origin;
    private float lifespan;
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
        if(lifespan <= 0)
        {
            // Ǯ�Ŵ���.Destroy
        }
    }

    protected override void MyDestroy()
    {
        // �ı��� �Ŀ� �ʱ�ȭ
        lifespan = -1;
    }

    public void SetInfo(ResourceEnum.Prefab wantOrigin, Queue<GameObject> wantQueue) 
    { 
        origin = wantOrigin;
        originPool = wantQueue;
    }
}
