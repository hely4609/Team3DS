using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KYH_Test : MonoBehaviour
{
    void Start()
    {
        GameManager.ManagerStarts += (() =>
        {
            GameManager.Instance.UIManager.Open(UIEnum.SignInCanvas);
        });
        
    }

}
