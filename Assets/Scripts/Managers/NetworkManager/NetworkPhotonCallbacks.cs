using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NetworkPhotonCallbacks : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;

    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            //Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            Vector3 spawnPosition = new Vector3(0, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);
            
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
        GameManager.Instance.UIManager.ClaimError("Shutdowned", shutdownReason.ToString(), "OK");
    }
    public void OnConnectedToServer(NetworkRunner runner) 
    {
        Debug.Log("Connected to server");
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) 
    { 
        GameManager.Instance.UIManager.ClaimError("Disconneccted", reason.ToString(), "OK");
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) 
    { 
        GameManager.Instance.UIManager.ClaimError("Connect failed", reason.ToString(), "OK");

    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"Session List Updated with {sessionList.Count} session(s)");

        // Example
        // Search the list for a Session with a specific Property

        // Store the target session
        SessionInfo session = null;

        foreach (var sessionItem in sessionList)
        {

            // Check for a specific Custom Property
            if (sessionItem.Properties.TryGetValue("type", out var propertyType) && propertyType.IsInt)
            {

                var gameType = (int)propertyType.PropertyValue;
                Debug.Log($"gameType : {gameType}");
                // Check for the desired Game Type
                if (gameType == 1)
                {

                    // Store the session info
                    session = sessionItem;
                    break;
                }
            }
        }

        // Check if there is any valid session
        if (session != null)
        {
            Debug.Log($"Joining {session.Name}");

            // Join
            runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client, // Client GameMode, could be Shared as well
                SessionName = session.Name, // Session to Join
                                            // ...
            });
        }
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
