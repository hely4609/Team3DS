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
            Debug.Log("회원가입 성공! " + bro);
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
            Debug.Log("로그인 성공 " + bro);
            GameManager.Instance.NetworkManager.currentNetworkState = NetworkState.SignIn;
            GameManager.Instance.UIManager.Close(UIEnum.SignInCanvas);
        }
        else
        {
            GameManager.Instance.UIManager.ClaimError(bro.GetErrorCode(), bro.GetMessage(), "OK");
        }
    }

    public static void UpdateNickname(string inputNickname)
    {
        var bro = Backend.BMember.UpdateNickname(inputNickname);
        if(bro.IsSuccess())
        {
            GameManager.Instance.UIManager.Close(UIEnum.SetNicknameCanvas);
            GameManager.Instance.UIManager.ClaimError("Success", "Nickname has been changed successfully", "OK");
        }
        else
        {
            GameManager.Instance.UIManager.ClaimError(bro.GetErrorCode(), bro.GetMessage(), "OK");

        }
    }
}
