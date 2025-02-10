using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class NetworkPhotonCallbacks : MonoBehaviour, INetworkRunnerCallbacks
{
    //protected static NetworkPhotonCallbacks instance;

    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //        //DontDestroyOnLoad(this);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}


    [SerializeField] private NetworkPrefabRef _playerPrefab;

    public static bool[] playerArray = new bool[4];
    
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    public Dictionary<PlayerRef, NetworkObject> SpawnedCharacter => _spawnedCharacters;
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            StartCoroutine(GameManager.Instance.GameStart());
            GameManager.ClaimLoadInfo("Joining room");
        }   
        if (runner.IsServer)
        {
            // Create a unique position for the player
            //Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            Vector3 spawnPosition = new Vector3(0, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            //networkPlayerObject.GetComponent<ControllerBase>().myAuthority = player;

            ControllerBase con = networkPlayerObject.GetComponent<ControllerBase>();
            int playerNumber = System.Array.FindIndex(playerArray, target => target == false);
            playerArray[playerNumber] = true;
            con.MyNumber = playerNumber;

            if (networkPlayerObject.HasInputAuthority)
            {
                GameManager.Instance.NetworkManager.LocalController = con;
            }

            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);

            //foreach (var building in GameManager.Instance.BuildingManager.Buildings)
            //{
            //    building.BuildingTimeCurrent += 0.001f;
            //}
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) 
    { 
        if(GameManager.IsGameStart)GameManager.Instance.GameOver();
        if (!GameManager.Instance.IsDefeated && !GameManager.Instance.IsClear)
        {
            GameManager.Instance.UIManager.ClaimError("Shutdowned", "The server has been disconnected.", "확인", () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
    }
    public void OnConnectedToServer(NetworkRunner runner) 
    {
        Debug.Log("Connected to server");
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) 
    { 
        GameManager.Instance.GameOver();
        GameManager.Instance.UIManager.ClaimError("Disconneccted", "The network connection has been lost.", "확인", () => { 
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
            runner.Shutdown();
        });
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) 
    { 
        GameManager.Instance.GameOver();
        GameManager.Instance.UIManager.ClaimError("Connect failed", "Network connection failed.", "확인", () => {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
            runner.Shutdown();
        });

    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public static List<SessionInfo> sessionList = new List<SessionInfo>();
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"Session List Updated with {sessionList.Count} session(s)");
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
