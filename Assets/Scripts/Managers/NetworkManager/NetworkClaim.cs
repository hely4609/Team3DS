using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using BackEnd.Tcp;
using UnityEngine.SceneManagement;
using System;

public partial class NetworkManager : Manager
{

    public static void ClaimSignUp(string  inputID, string inputPassword) 
    {
    }

    public static IEnumerator SignUpStart(string inputID, string inputPassword)
    {
        BackendReturnObject bro = null;
        yield return new WaitForFunction(() =>
        {
            bro = Backend.BMember.CustomSignUp(inputID, inputPassword);
        });
        if(bro.IsSuccess())
        {
            Debug.Log("회원가입 성공! " + bro);
            GameManager.Instance.UIManager.ClaimError("Success", "Signed up successfully", "OK");
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
                UserInfo gotInfo = new()
                {
                    gamerId = userInfoJson["row"]["gamerId"].ToString(),
                    nickname = userInfoJson["row"]["nickname"]?.ToString()
                };
                GameManager.Instance.NetworkManager.myInfo = gotInfo;

                if (GameManager.Instance.NetworkManager.MyNickname == null) GameManager.ManagerStarts += () => GameManager.Instance.UIManager.Open(UIEnum.SetNicknameCanvas);
                
                yield return new WaitWhile(() => GameManager.Instance.NetworkManager.MyNickname == null);
                yield return MatchMakingServerStart();
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

    public static void ClaimMatchMakingServer()
    {
        GameManager.Instance.StartCoroutine(MatchMakingServerStart());
    }

    public static IEnumerator MatchMakingServerStart()
    {
        ErrorInfo errorInfo;
        yield return new WaitForFunction(() =>
        {
            if(!Backend.Match.JoinMatchMakingServer(out errorInfo))
            {
                GameManager.ManagerStarts += () => GameManager.Instance.UIManager.ClaimError(errorInfo.Category.ToString(), errorInfo.Reason.ToString(), "OK", ()=>SceneManager.LoadScene(0));
            }
        });
        // 서버에 접속했으면 델리게이트에 매치폴 넣어주기!(중요)
        GameManager.NetworkUpdates -= (deltaTime) => Backend.Match.Poll();
        GameManager.NetworkUpdates += (deltaTime) => Backend.Match.Poll();

        BackendReturnObject gotMatchCards = null;
        yield return new WaitForFunction(() =>
        {
            gotMatchCards = Backend.Match.GetMatchList();
        });
        if(!gotMatchCards.IsSuccess())
        {
            GameManager.ManagerStarts += () => GameManager.Instance.UIManager.ClaimError(gotMatchCards.GetErrorCode(), gotMatchCards.GetMessage(), "OK", () => SceneManager.LoadScene(0));
        }
        
        List<MatchCard> matchCards = new();
        JsonData matchCardsJson = gotMatchCards.FlattenRows();
        foreach (JsonData currentRow in matchCardsJson)
        {
            MatchCard card = new()
            {
                inDate = currentRow["inDate"].ToString(),
                matchHeadCount = int.Parse(currentRow["matchHeadCount"].ToString()),
                matchTitle = currentRow["title"].ToString(),
            };
            matchCards.Add(card);
        }
        GameManager.Instance.NetworkManager.matchCardArray = matchCards.ToArray();
        Debug.Log("매치카드 로딩 성공");
    }

    public static void ClaimMakeRoom()
    {
        GameManager.Instance.StartCoroutine(MakeRoomStart());
    }

    public static IEnumerator MakeRoomStart()
    {
        yield return new WaitForFunction(() =>
        {
            Backend.Match.CreateMatchRoom();
        });
        LobbyScript lobbyScript = GameObject.FindAnyObjectByType<LobbyScript>();
        lobbyScript.OpenRoom();
    }

    public static void ClaimInvite(string nickname)
    {
        GameManager.Instance.StartCoroutine(InvateStart(nickname));
    }

    public static IEnumerator InvateStart(string nickname)
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
}
