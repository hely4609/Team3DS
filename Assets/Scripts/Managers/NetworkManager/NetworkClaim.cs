using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;

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
        GameManager.Instance.StartCoroutine(SignInStart(inputID, inputPassword));
    }
    public static IEnumerator SignInStart(string inputID, string inputPassword)
    {
        BackendReturnObject bro = null;
        yield return new WaitForFunction(() =>
        {
            bro = Backend.BMember.CustomLogin(inputID, inputPassword);
        });

        if(bro.IsSuccess())
        {
            Debug.Log("로그인 성공 " + bro);
            GameManager.Instance.NetworkManager.currentNetworkState = NetworkState.SignIn;
            GameManager.ManagerStarts += () => GameManager.Instance.UIManager.Close(UIEnum.SignInCanvas);
            BackendReturnObject getUserInfoResult = null;
            yield return new WaitForFunction(() =>
            {
                getUserInfoResult = Backend.BMember.GetUserInfo();
            });
            if(getUserInfoResult.IsSuccess()) 
            {
                JsonData userInfoJson = getUserInfoResult.GetReturnValuetoJSON();
                UserInfo gotInfo = new();
                gotInfo.gamerId = userInfoJson["row"]["gamerId"].ToString();
                gotInfo.nickname = userInfoJson["row"]["nickname"]?.ToString();
                GameManager.Instance.NetworkManager.myInfo = gotInfo;

                if (GameManager.Instance.NetworkManager.MyNickname == null) GameManager.ManagerStarts += () => GameManager.Instance.UIManager.Open(UIEnum.SetNicknameCanvas);
            }
            else
            {
                GameManager.ManagerStarts += () => GameManager.Instance.UIManager.ClaimError(getUserInfoResult.GetErrorCode(), getUserInfoResult.GetMessage(), "OK");
            }
        }
        else
        {
            GameManager.Instance.UIManager.ClaimError(bro.GetErrorCode(), bro.GetMessage(), "OK");
        }
    }

    public static void UpdateNickname(string inputNickname)
    {
        GameManager.Instance.StartCoroutine(UpdateNicknameStart(inputNickname));
    }

    public static IEnumerator UpdateNicknameStart(string inputNickname)
    {
        BackendReturnObject bro = null;
        yield return new WaitForFunction(() =>
        {
            bro = Backend.BMember.UpdateNickname(inputNickname);

        });
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
