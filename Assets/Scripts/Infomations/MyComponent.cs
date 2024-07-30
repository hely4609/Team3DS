using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyComponent : NetworkBehaviour
{
    // ������� pooling���� �����Ұ͵� ��������Ʈ�� ���, ���ֱ� �ϴ� ����
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
