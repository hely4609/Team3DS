using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSignCanvas : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] RectTransform canvas0;
    [SerializeField] RectTransform canvas1;
    [SerializeField] RectTransform canvas2;
    [SerializeField] RectTransform canvas3;

    float rate = 100;
    public void SetRadius(float radius)
    {
        this.radius = radius;
        canvas0.ForceUpdateRectTransforms();
        canvas0.localPosition = new Vector3(0, 0, radius * rate);
        canvas1.localPosition = new Vector3(0, 0, -radius * rate);
        canvas2.localPosition = new Vector3(radius * rate, 0, 0);
        canvas3.localPosition = new Vector3(-radius * rate, 0, 0);
    }

    private void Update()
    {
        if(gameObject.activeSelf)
        {
            transform.Rotate(new Vector3(0, 1, 0));
        }
    }
}
