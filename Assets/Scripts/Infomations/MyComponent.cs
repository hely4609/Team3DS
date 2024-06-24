using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    // ������� pooling���� �����Ұ͵� ��������Ʈ�� ���, ���ֱ� �ϴ� ����
    protected virtual void MyStart() { }
    protected virtual void MyUpdate(float deltaTime) {}
    protected virtual void MyDestroy() {}

    protected void OnEnable() { }
    protected void OnDisable() { }
}
