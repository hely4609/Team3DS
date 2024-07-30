using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyComponent : NetworkBehaviour
{
    // 생성대신 pooling으로 관리할것들 델리게이트에 등록, 빼주기 하는 역할
    protected virtual void MyStart() { GameManager.ObjectUpdates += MyUpdate; }
    protected virtual void MyUpdate(float deltaTime) {}
    protected virtual void MyDestroy() {}

    protected virtual void OnEnable()
    {
        GameManager.ObjectStarts += MyStart;
        //GameManager.ObjectUpdates += MyUpdate;
    }
    protected virtual void OnDisable()
    {
        GameManager.ObjectDestroies -= MyDestroy;
        GameManager.ObjectDestroies += MyDestroy;
        GameManager.ObjectUpdates -= MyUpdate;
        GameManager.ObjectStarts -= MyStart;
    }
}
