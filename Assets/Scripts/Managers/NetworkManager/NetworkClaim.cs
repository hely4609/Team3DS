using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using BackEnd.Tcp;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;
using Photon.Pun;

public partial class NetworkManager : Manager
{

    public static void ClaimSignUp(string  inputID, string inputPassword) 
    {
        GameManager.Instance.StartCoroutine(SignUp(inputID, inputPassword));
    }

    public static IEnumerator SignUp(string inputID, string inputPassword)
    {
        BackendReturnObject bro = null;
        yield return new WaitForFunction(() =>
        {
            bro = Backend.BMember.CustomSignUp(inputID, inputPassword);
        });
        if(bro.IsSuccess())
        {
            Debug.Log("회원가입 성공! " + bro);
            SignInCanvas signin = GameObject.FindAnyObjectByType<SignInCanvas>();
            signin.CloseSignUp();
            GameManager.Instance.UIManager.ClaimError("Success", "Signed up successfully", "OK");
        }
        else
        {
            GameManager.Instance.UIManager.ClaimError(bro.GetErrorCode(), bro.GetMessage(), "OK");
        }
    }

    public static void ClaimSignIn(string inputID,  string inputPassword)
    {
        GameManager.Instance.StartCoroutine(SignIn(inputID, inputPassword));
    }
    public static IEnumerator SignIn(string inputID, string inputPassword)
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
                UserInfo gotInfo = new()
                {
                    gamerId = userInfoJson["row"]["gamerId"].ToString(),
                    nickname = userInfoJson["row"]["nickname"]?.ToString()
                };
                GameManager.Instance.NetworkManager.myInfo = gotInfo;

                if (GameManager.Instance.NetworkManager.MyNickname == null) GameManager.ManagerStarts += () => GameManager.Instance.UIManager.Open(UIEnum.SetNicknameCanvas);
                
                yield return new WaitWhile(() => GameManager.Instance.NetworkManager.MyNickname == null);
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

    public static void ClaimUpdateNickname(string inputNickname)
    {
        GameManager.Instance.StartCoroutine(UpdateNickname(inputNickname));
    }

    public static IEnumerator UpdateNickname(string inputNickname)
    {
        yield return new WaitForFunction(() =>
        {
            PhotonNetwork.NickName = inputNickname;
        });
        // 임시
        UserInfo gotInfo = new()
        {
            gamerId = null,
            nickname = inputNickname
        };
        GameManager.Instance.NetworkManager.myInfo = gotInfo;
        
        GameManager.Instance.UIManager.Close(UIEnum.SetNicknameCanvas);
        GameManager.Instance.UIManager.ClaimError("Success", "Nickname has been changed successfully", "OK");

    }

    public static void ClaimCreateRoom()
    {
        GameManager.Instance.StartCoroutine(CreateRoom());
    }

    public static IEnumerator CreateRoom()
    {
        yield return new WaitForFunction(() =>
        {
            PhotonNetwork.CreateRoom("Test");
        });

    }

    public static void ClaimInvite(string nickname)
    {
        GameManager.Instance.StartCoroutine(Invate(nickname));
    }

    public static IEnumerator Invate(string nickname)
    {
        yield return new WaitForFunction(() =>
        {
            Backend.Match.InviteUser(nickname);
        });
    }

    public static void ClaimAcceptInvite(SessionId roomId, string roomToken)
    {
        GameManager.Instance.StartCoroutine(AcceptInvite(roomId, roomToken));
    }

    public static IEnumerator AcceptInvite(SessionId roomId, string roomToken)
    {
        yield return new WaitForFunction(() =>
        {
            Backend.Match.AcceptInvitation(roomId, roomToken);
        });
    }

    public static void ClaimRejectInvite(SessionId roomId, string roomToken)
    {
        GameManager.Instance.StartCoroutine(RejectInvite(roomId, roomToken));
    }

    public static IEnumerator RejectInvite(SessionId roomId, string roomToken)
    {
        yield return new WaitForFunction(() =>
        {
            Backend.Match.DeclineInvitation(roomId, roomToken);
        });
    }

    public static void ClaimLeaveRoom()
    {
        GameManager.Instance.StartCoroutine(LeaveRoom());
    }

    public static IEnumerator LeaveRoom()
    {
        yield return new WaitForFunction(() =>
        {
            PhotonNetwork.LeaveRoom();
        });
    }

    public static void ClaimMatchMaking()
    {

    }

    public static IEnumerator MatchMaking()
    {
        yield return null;
    }
}
