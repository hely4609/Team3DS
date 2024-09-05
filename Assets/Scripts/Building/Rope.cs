using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public void Initializer(float scale)
    {
        transform.localScale = new Vector3(1,1,scale);
    }
}
