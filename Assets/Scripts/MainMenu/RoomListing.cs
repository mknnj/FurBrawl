using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField] private Text _text; //the content of the listing

    public RoomInfo RoomInfo { get; private set; } 
    public void SetRoomInfo(RoomInfo roomInfo) //used from roomlistingmenu to keep track of the number of players and the room name
    {
        RoomInfo = roomInfo;
        _text.text = roomInfo.PlayerCount.ToString() + '/' + roomInfo.MaxPlayers + ", " +roomInfo.Name;
    }

    public void OnClick_Room()
    {
        PhotonNetwork.JoinRoom(RoomInfo.Name);
    }
}
