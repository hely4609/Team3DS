using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;
using Fusion;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

// 네트워크 백엔드 : 뒤끝 -> 포톤
public enum NetworkState
{
    Offline, Initiating, Connected, SignIn,
    OnMatchingServer, OnMatchRoom, Matching,
    JoinGameServer, OnGameServer, OnGameRoom,
    WorldJoin, Disconnected
}
public partial class NetworkManager : Manager
{
    private GameObject _runner;
    public NetworkRunner Runner
    {
        get
        {
            if(_runner) return _runner.GetComponent<NetworkRunner>();
            else return null;
        }

    }
    NetworkState currentNetworkState = NetworkState.Offline;
    public NetworkState CurrentNetworkState => currentNetworkState;

    string gameVersion = "1";

    public class UserInfo
    {
        public string gamerId;
        public string nickname;
    }

    public class MatchCard
    {
        public string inDate;
        public MatchType matchType;
        public MatchModeType matchModeType;
    }

    public MatchCard[] matchCardArray;

    UserInfo myInfo;
    public string MyNickname
    {
        get => myInfo?.nickname;
        set
        {
            if (myInfo != null) return;
            else
            {
                myInfo.nickname = value;
            }
        }
    }
    
    public override IEnumerator Initiate()
    {
        GameManager.ClaimLoadInfo("Network Initializing");

        // Create the Fusion runner and let it know that we will be providing user input
        _runner = new GameObject("NetworkRunner");
        _runner.AddComponent<NetworkRunner>();
        Runner.ProvideInput = true;

        currentNetworkState = NetworkState.Initiating;

        GameManager.NetworkUpdates += (deltaTime) => {
            if(!Runner)
            {
                _runner = new GameObject("NetworkRunner");
                _runner.AddComponent<NetworkRunner>();
            }
        };

        yield return null;
    }

    public IEnumerator StartGame(GameMode mode)
    {
        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        yield return new WaitForFunction(()=> Runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = GameManager.Instance.AddComponent<NetworkSceneManagerDefault>()
        }));

    }
}
