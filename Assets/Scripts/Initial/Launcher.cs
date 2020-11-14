using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject ConnectedScreen; // a UI object under which there are the UI elements responsible for connecting to a room/ joining a room
    public GameObject DisconnectedScreen; // a simple RED UI interface shows that we've disconnected 
    /*DisconnectedScreen can be furthe improved to have a 'reconnect' button*/

    public string Error; //the error message (not being used, yet)
    public void  OnClick_ConnectBtn()
    {
        PhotonNetwork.SendRate = 40;
        PhotonNetwork.ConnectUsingSettings(); //uses the API_ID from settings to connect to Photon Server
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default); //a mandatory extension to ConnectUsingSettings
    }

    public override void OnJoinedLobby() // 
    {
        if (DisconnectedScreen.activeSelf)
            DisconnectedScreen.SetActive(false);
        ConnectedScreen.SetActive(true);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        //TODO [Sorre97] inform EnvironmentManager of the disconnection
        DisconnectedScreen.SetActive(true);
        Error = cause.ToString();
    }
}
