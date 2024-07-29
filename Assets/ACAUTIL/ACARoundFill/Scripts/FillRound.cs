using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillRound : MonoBehaviour
{
    float distance;
    public Image ImgFill, ImgStartDot, ImgEndDot;
    public ImgsFillDynamic ImgsFD;
    private void Awake()
    {
        this.distance = Vector3.Distance(this.transform.position, this.ImgStartDot.transform.position);
    }

    public void SetFill(float _amount)
    {
        this.ImgFill.fillAmount = _amount;
        this.RefreshAngle();
    }

    void RefreshAngle()
    {
        float ratio = this.ImgsFD != null ? this.ImgsFD.Factor : this.ImgFill.fillAmount;

        this.ImgStartDot.transform.localPosition = Vector3.zero;
        this.ImgStartDot.transform.rotation = Quaternion.identity;
        this.ImgStartDot.transform.Translate(0, this.distance, 0);

        this.ImgEndDot.transform.localPosition = Vector3.zero;
        this.ImgEndDot.transform.rotation = Quaternion.identity;
        this.ImgEndDot.transform.Rotate(0, 0, -this.GetAngle(ratio), Space.Self);
        this.ImgEndDot.transform.Translate(0, this.distance, 0, Space.Self);
    }

    float GetAngle(float _amount)
    {
        return _amount * 360F;
    }
}
