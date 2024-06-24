using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingInfo : MyComponent
{
    private ResourceEnum.Prefab origin;
    public ResourceEnum.Prefab Origin => origin;
    private float lifespan;
    public float Lisfespan { get; set; }

    protected override void MyUpdate(float deltaTime)
    {
        base.MyUpdate(deltaTime);
    }

    protected override void MyDestroy()
    {
        base.MyDestroy();
    }

    public void SetInfo(ResourceEnum.Prefab wantOrigin) { }
}
