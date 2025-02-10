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
    public static void ClaimUpdateNickname(string inputNickname)
    {
        GameManager.Instance.StartCoroutine(UpdateNickname(inputNickname));
    }

    public static IEnumerator UpdateNickname(string inputNickname)
    {
        yield return null;

        // 임시
        UserInfo gotInfo = new()
        {
            gamerId = null,
            nickname = inputNickname
        };
        GameManager.Instance.NetworkManager.myInfo = gotInfo;
        
        GameManager.Instance.UIManager.Close(UIEnum.SetNicknameCanvas);
        GameManager.Instance.UIManager.ClaimError("Success", "Nickname has been changed successfully", "확인");

    }

    public static void ClaimTutorial()
    {
        _ = Tutorial(GameManager.Instance.NetworkManager.Runner, GameMap.Default, GameType.Default);
    }

    public static async Task Tutorial(NetworkRunner runner, GameMap gameMap, GameType gameType)
    {
        var customProps = new Dictionary<string, SessionProperty>();
        customProps["map"] = (int)gameMap;
        customProps["type"] = (int)gameType;

        GameManager.ClaimLoadInfo("Entering game");
        var result = await runner.StartGame(new StartGameArgs()
        {
            //SessionName = $"{DateTime.Now.ToString("mmss")}",
            GameMode = GameMode.Single,
            SessionProperties = customProps,
        });
        GameManager.ClaimLoadInfo("Entering game", 1, 2);

        if (result.Ok)
        {
            GameObject.Find("LobbyCanvas").SetActive(false);
        }
        else
        {
            GameManager.Instance.UIManager.ClaimError("Failed to Start", "Room creation failed.", "확인", () => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });
        }
        GameManager.ClaimLoadInfo("Entering game", 2, 2);

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
            //SessionName = $"{DateTime.Now.ToString("mmss")}",
            GameMode = GameMode.Host,
            SessionProperties = customProps,
        });
        GameManager.ClaimLoadInfo("Entering game", 1, 2);

        if (result.Ok)
        {
            // all good
            //await Task.Delay(3000);
            GameObject.Find("LobbyCanvas").SetActive(false);
        }
        else
        {
            GameManager.Instance.UIManager.ClaimError("Failed to Start", "Room creation failed.", "확인", () => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });
        }
        GameManager.ClaimLoadInfo("Entering game", 2, 2);
        
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
        }
        else
        {
            GameManager.Instance.UIManager.ClaimError(result.ShutdownReason.ToString(), result.ErrorMessage, "확인", () => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });
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
        GameManager.ClaimLoadInfo("Entering game");
        //var result = await runner.JoinSessionLobby(SessionLobby.Custom, roomName);
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = roomName,
        });
        GameManager.ClaimLoadInfo("Entering game", 1, 2);
        if (result.Ok)
        {
            // all good
            GameObject.Find("LobbyCanvas").SetActive(false);

        }
        else
        {
            GameManager.Instance.UIManager.ClaimError(result.ShutdownReason.ToString(), "No room matches the number you entered.", "확인", () => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });
        }
        GameManager.ClaimLoadInfo("Entering game", 2, 2);
        GameManager.CloseLoadInfo();
    }

}
