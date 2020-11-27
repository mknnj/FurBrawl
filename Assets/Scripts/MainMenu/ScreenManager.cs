using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviourPunCallbacks
{
    //Following the structure defined in the final game design document:
    public GameObject TitleScreen; //Simple screen with "play" button at the start of the game
    public GameObject MainMenu; //Menu of the game where one can choose among "settings", "play" or "commands tutorial" 
    public GameObject RoomListMenu; //Menu listing available rooms where you can create/join rooms
    public GameObject InRoomScreen; //Lists the players in the room and their status (ready/not ready, skins, etc)
    public GameObject SettingsMenu; //Maybe used later
    public GameObject TutorialScreen; //Explains how to play the game
    public GameObject DisconnectedScreen; //Used to display errors
    
    
    public string Error; //the error message (not being used, yet)
    public InputField NickNameTF;
    
    public void  OnClick_ConnectToRoomsBtn()
    {
        if(!NickNameTF.text.Equals("")) //Maybe we should display some UI to notify that the user needs to choose a nickname
        {
            PhotonNetwork.NickName = NickNameTF.text; //sets the nickname of the player
            PhotonNetwork.SendRate = 40;
            PhotonNetwork.ConnectUsingSettings(); //uses the API_ID from settings to connect to Photon Server
        }
    }

    public void OnClick_PlayBtn() //go to the main menu
    {
        if (TitleScreen.activeSelf)
            TitleScreen.SetActive(false);
        MainMenu.SetActive(true);
    }
    
    public void OnClick_SettingsBtn() //jump to the settings menu
    {
        if (MainMenu.activeSelf)
            MainMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }
    
    public void OnClick_TutorialBtn() //jump to the tutorial
    {
        if (MainMenu.activeSelf)
            MainMenu.SetActive(false);
        TutorialScreen.SetActive(true);
    }

    public void OnClick_DisconnectBtn() // Disconnect from server
    {
        PhotonNetwork.Disconnect();
    }
    
    public void OnClick_ReturnToMain() //now the same for settings/tutorial, maybe we want to save settings or something
    {
        SettingsMenu.SetActive(false);
        TutorialScreen.SetActive(false);
        DisconnectedScreen.SetActive(false);
        MainMenu.SetActive(true);
    }

    public override void OnConnectedToMaster() 
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default); //a mandatory extension to ConnectUsingSettings
    }

    public override void OnJoinedLobby() // When the client joins the lobby, enter the room list menu
    {
        DisconnectedScreen.SetActive(false); //The client can arrive from a reconnection
        MainMenu.SetActive(false); //The client can arrive from the main menu
        RoomListMenu.SetActive(true); //When the client is connected, it should be able to see the rooms
    }
    
    public override void OnJoinedRoom()
    {
        RoomListMenu.SetActive(false);
        InRoomScreen.SetActive(true);
        print("JOINED");
        //PhotonNetwork.LoadLevel(1);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("FAILED TO JOIN ROOM");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        RoomListMenu.SetActive(false); //Upon disconnection, one should not see anymore the list of rooms
        InRoomScreen.SetActive(false); 
        DisconnectedScreen.SetActive(true);
        Error = cause.ToString();
    }
    
}
