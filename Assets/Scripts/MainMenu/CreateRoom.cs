using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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
        PhotonNetwork.CreateRoom(CreateRoomTF.text, new Photon.Realtime.RoomOptions { MaxPlayers = 4 }, null);
        Debug.Log("Created room "+CreateRoomTF.text);
    }

    
}
