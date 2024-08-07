using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public enum GameType : int
{
    Default,
}

public enum GameMap : int
{
    Default,
}
public partial class NetworkManager : Manager
{

    public static void ClaimSignUp(string  inputID, string inputPassword) 
    {
        GameManager.Instance.StartCoroutine(SignUp(inputID, inputPassword));
    }

    public static IEnumerator SignUp(string inputID, string inputPassword)
    {
        yield return null;
    }

    public static void ClaimSignIn(string inputID,  string inputPassword)
    {
        GameManager.Instance.StartCoroutine(SignIn(inputID, inputPassword));
    }
    public static IEnumerator SignIn(string inputID, string inputPassword)
    {
        yield return null;
    }

    public static void ClaimUpdateNickname(string inputNickname)
    {
        GameManager.Instance.StartCoroutine(UpdateNickname(inputNickname));
    }

    public static IEnumerator UpdateNickname(string inputNickname)
    {
        yield return null;

        // юс╫ц
        UserInfo gotInfo = new()
        {
            gamerId = null,
            nickname = inputNickname
        };
        GameManager.Instance.NetworkManager.myInfo = gotInfo;
        
        GameManager.Instance.UIManager.Close(UIEnum.SetNicknameCanvas);
        GameManager.Instance.UIManager.ClaimError("Success", "Nickname has been changed successfully", "OK");

    }

    public static void ClaimStartHost()
    {
        _ = StartHost(GameManager.Instance.NetworkManager.Runner, GameMap.Default, GameType.Default);
    }

    public static async Task StartHost(NetworkRunner runner, GameMap gameMap, GameType gameType)
    {
        var customProps = new Dictionary<string, SessionProperty>();
        customProps["map"] = (int)gameMap;
        customProps["type"] = (int)gameType;

        GameManager.ClaimLoadInfo("Entering game");
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionProperties = customProps,
            //CustomLobbyName = $"{GameManager.Instance.NetworkManager.MyNickname}'s lobby",
        });
        GameManager.ClaimLoadInfo("Entering game", 1, 2);

        if (result.Ok)
        {
            // all good
            //await Task.Delay(3000);
            GameObject.Find("LobbyCanvas").SetActive(false);
            GameManager.Instance.GameStart();
        }
        else
        {
            GameManager.Instance.UIManager.ClaimError("Failed to Start", result.ShutdownReason.ToString(), "OK", () => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });
        }
        GameManager.ClaimLoadInfo("Entering game", 2, 2);
        GameManager.CloseLoadInfo();

    }

    public static void ClaimInvite(string nickname)
    {
        GameManager.Instance.StartCoroutine(Invate(nickname));
    }

    public static IEnumerator Invate(string nickname)
    {
        yield return null;
    }

    public static void ClaimAcceptInvite()
    {
        GameManager.Instance.StartCoroutine(AcceptInvite());
    }

    public static IEnumerator AcceptInvite()
    {
        yield return null;
    }

    public static void ClaimRejectInvite()
    {
        GameManager.Instance.StartCoroutine(RejectInvite());
    }

    public static IEnumerator RejectInvite()
    {
        yield return null;
    }

    public static void ClaimLeaveRoom()
    {
        GameManager.Instance.StartCoroutine(LeaveRoom());
    }

    public static IEnumerator LeaveRoom()
    {
        yield return null;
    }

    public static void ClaimJoinRandomRoom()
    {
        _ = JoinRandomRoom(GameManager.Instance.NetworkManager.Runner, GameMap.Default, GameType.Default);
    }

    public static async Task JoinRandomRoom(NetworkRunner runner, GameMap gameMap, GameType gameType)
    {
        var customProps = new Dictionary<string, SessionProperty>();
        customProps["map"] = (int)gameMap;
        customProps["type"] = (int)gameType;

        GameManager.ClaimLoadInfo("Entering game");
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionProperties = customProps,
        });

        GameManager.ClaimLoadInfo("Entering game", 1, 2);
        if (result.Ok)
        {
            // all good
            GameObject.Find("LobbyCanvas").SetActive(false);
            GameManager.Instance.GameStart();
        }
        else
        {
            GameManager.Instance.UIManager.ClaimError(result.ShutdownReason.ToString(), result.ErrorMessage, "OK", () => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });
        }
        GameManager.ClaimLoadInfo("Entering game", 2, 2);
        GameManager.CloseLoadInfo();
    }

    public static void ClaimJoinRoom(string roomName)
    {
        _ = JoinRoom(GameManager.Instance.NetworkManager.Runner, roomName);
    }

    public static async Task JoinRoom(NetworkRunner runner, string roomName)
    {
        var result = await runner.JoinSessionLobby(SessionLobby.Custom, roomName);

        if (result.Ok)
        {
            // all good
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public static void ClaimMatchMaking()
    {

    }

    public static IEnumerator MatchMaking()
    {
        yield return null;
    }
}
