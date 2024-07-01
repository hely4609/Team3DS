using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public partial class NetworkManager : Manager
{

    public static void ClaimSignUp(string  inputID, string inputPassword) 
    {
        var bro = Backend.BMember.CustomSignUp(inputID, inputPassword);
        if(bro.IsSuccess())
        {
            Debug.Log("ȸ������ ����! " + bro);
        }
        else
        {
            GameManager.Instance.UIManager.ClaimError(bro.GetErrorCode(), bro.GetMessage(), "OK");
        }
    }

    public static void ClaimSignIn(string inputID,  string inputPassword)
    {
        var bro = Backend.BMember.CustomLogin(inputID, inputPassword);
        if(bro.IsSuccess())
        {
            Debug.Log("�α��� ���� " + bro);
        }
        else
        {
            GameManager.Instance.UIManager.ClaimError(bro.GetErrorCode(), bro.GetMessage(), "OK");
        }
    }
}
