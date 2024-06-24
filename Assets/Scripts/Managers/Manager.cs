using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager
{
    public virtual IEnumerator Initiate() { return default; }
    public virtual void ManagerStart() { }
    public virtual void ManagerUpdate(float deltaTime) { }
}
