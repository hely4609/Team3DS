using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class towertest : Tower
{
    [SerializeField] Renderer[] render;
    [Range(0,10)]
    [SerializeField] float value;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        foreach (var r in render) 
        {
            r.material.SetFloat("_CompletValue", value);
        }
        
    }
}
