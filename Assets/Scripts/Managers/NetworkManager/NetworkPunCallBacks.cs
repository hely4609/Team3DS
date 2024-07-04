using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkPunCallBacks : MonoBehaviourPunCallbacks
{
    // 서버 연결
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        GameManager.Instance.UIManager.ClaimError("Disconnected from server", cause.ToString(), "OK", () => SceneManager.LoadScene(0));
    }

    // 방 생성
    public override void OnCreatedRoom()
    {
        Debug.Log("방생성됨");
        NetworkManager.numberOfPeopleInRoom = 1;
        LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
        lobby.OpenRoom(true);
        lobby.SetPlayerName(0, GameManager.Instance.NetworkManager.MyNickname);
        lobby.SetPlayerName(1, "");
        lobby.SetPlayerName(2, "");
        lobby.SetPlayerName(3, "");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        GameManager.Instance.UIManager.ClaimError(returnCode.ToString(), message.ToString(), "OK");
    }

    // 방입장
    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        GameManager.Instance.UIManager.ClaimError(returnCode.ToString(), message.ToString(), "OK");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        GameManager.Instance.UIManager.ClaimError("There are no room can join", "A new Room has been created", "OK");
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    // 남이 방에 들어옴
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }
    // 방 퇴장
    // 내가 퇴장
    public override void OnLeftRoom()
    {
        NetworkManager.numberOfPeopleInRoom = 0;
        LobbyScript lobby = GameObject.FindAnyObjectByType<LobbyScript>();
        lobby.CloseRoom();
    }

    // 남이 퇴장
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }


}
