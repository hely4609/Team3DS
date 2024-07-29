using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public ImgsFillDynamic ImgsFD;

    private void Update()
    {
        // Just Test >___<~*
        if (Input.GetKeyDown(KeyCode.Space))
            this.ImgsFD.SetValue(Random.Range(0F, 1F), false, Random.Range(1F, 2F));

       

        if (Input.GetKeyDown(KeyCode.Alpha1))
            this.ImgsFD.SetValue(0.3f, false, Random.Range(0.01f, 0.5f));
        if (Input.GetKeyDown(KeyCode.Alpha2))
            this.ImgsFD.SetValue(0.6f, false, Random.Range(0.01f, 0.5f));
        if (Input.GetKeyDown(KeyCode.Alpha3))
            this.ImgsFD.SetValue(0.9f, false, Random.Range(0.01f, 0.5f));
        if (Input.GetKeyDown(KeyCode.Alpha4))
            this.ImgsFD.SetValue(1F, false, Random.Range(0.01f, 0.5f));

        if (Input.GetKeyDown(KeyCode.Alpha5))
            this.ImgsFD.SetValue(1F, false, 0.001f);

        if (Input.GetKeyDown(KeyCode.Alpha0))
            this.ImgsFD.SetValue(0F, true);

    }

}
