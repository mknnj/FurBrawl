using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class CreateRoom: MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField CreateRoomTF;
    [SerializeField] private InputField JoinRoomTF;

    public void OnClick_JoinRoom() {
        PhotonNetwork.JoinRoom(JoinRoomTF.text, null);
    }
    public void OnClick_CreateRoom()
    {
        if (!PhotonNetwork.IsConnected) return;
        PhotonNetwork.CreateRoom(CreateRoomTF.text, new Photon.Realtime.RoomOptions { MaxPlayers = 4 , BroadcastPropsChangeToAll = true}, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room "+CreateRoomTF.text);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room:  "+message);
    }
}
