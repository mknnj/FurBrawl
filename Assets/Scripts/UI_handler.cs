using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_handler : MonoBehaviourPunCallbacks
{

    public InputField CreateRoomTF;
    public InputField JoinRoomTF;

    public void OnClick_JoinRoom() {

        PhotonNetwork.JoinRoom(JoinRoomTF.text, null);

    
    }
    public void OnClick_CreateRoom() {

        PhotonNetwork.CreateRoom(CreateRoomTF.text, new Photon.Realtime.RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom()
    {
        print("JOINED");
        PhotonNetwork.LoadLevel(1);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("FAILED TO JOIN ROOM");
    }
}
