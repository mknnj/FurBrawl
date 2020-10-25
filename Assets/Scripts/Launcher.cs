using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject ConnectedScreen;
    public GameObject DisconnectedScreen;
    public string Error;
    public void  OnClick_ConnectBtn()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        
    }

    public override void OnJoinedLobby()
    {
        if (DisconnectedScreen.activeSelf)
            DisconnectedScreen.SetActive(false);
        ConnectedScreen.SetActive(true);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectedScreen.SetActive(true);
        Error = cause.ToString();
    }
}
