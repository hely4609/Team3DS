using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    // 생성대신 pooling으로 관리할것들 델리게이트에 등록, 빼주기 하는 역할
    protected virtual void MyStart() { }
    protected virtual void MyUpdate(float deltaTime) {}
    protected virtual void MyDestroy() {}

    protected void OnEnable() { }
    protected void OnDisable() { }
}
