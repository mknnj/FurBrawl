using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScreenManager : MonoBehaviourPunCallbacks
{
    
    public enum Menu 
    {
        TitleScreen,
        MainMenu,
        RoomListMenu,
        InRoomScreen,
        SettingsMenu,
        TutorialScreen,
        DisconnectedScreen,
        FeedbackScreen,
        RulesScreen,
        VictoryScreen
    };
    
    //Following the structure defined in the final game design document:
    public GameObject TitleScreen; //Simple screen with "play" button at the start of the game
    public GameObject MainMenu; //Menu of the game where one can choose among "settings", "play" or "commands tutorial" 
    public GameObject RoomListMenu; //Menu listing available rooms where you can create/join rooms
    public GameObject InRoomScreen; //Lists the players in the room and their status (ready/not ready, skins, etc)
    public GameObject SettingsMenu; //Maybe used later
    public GameObject TutorialScreen; //Explains how to play the game
    public GameObject DisconnectedScreen; //Used to display errors
    public GameObject FeedbackScreen; //used to submit feedback to the professor's drive
    public GameObject RulesScreen;
    public GameObject VictoryScreen;
    
    public string Error; //the error message (not being used, yet)
    public InputField NickNameTF;

    public void Start()
    {
        if (CrossSceneVictoryInfo.GetWinner() == null)
            OpenTitleScreen();
        else
            OpenVictoryScreen();
    }

    public void SetMenu(Menu menu)
    {
        TitleScreen.SetActive(false);
        MainMenu.SetActive(false);
        RoomListMenu.SetActive(false);
        InRoomScreen.SetActive(false);
        SettingsMenu.SetActive(false);
        TutorialScreen.SetActive(false);
        DisconnectedScreen.SetActive(false);
        FeedbackScreen.SetActive(false);
        RulesScreen.SetActive(false);
        VictoryScreen.SetActive(false);
        switch (menu)
        {
            case Menu.TitleScreen:
                TitleScreen.SetActive(true);
                break;
            case Menu.MainMenu:
                MainMenu.SetActive(true);
                break;
            case Menu.RoomListMenu:
                RoomListMenu.SetActive(true);
                break;
            case Menu.InRoomScreen:
                InRoomScreen.SetActive(true);
                break;
            case Menu.SettingsMenu:
                SettingsMenu.SetActive(true);
                break;
            case Menu.TutorialScreen:
                TutorialScreen.SetActive(true);
                break;
            case Menu.DisconnectedScreen:
                DisconnectedScreen.SetActive(true);
                break;
            case Menu.FeedbackScreen:
                FeedbackScreen.SetActive(true);
                break;
            case Menu.RulesScreen:
                RulesScreen.SetActive(true);
                break;
            case Menu.VictoryScreen:
                VictoryScreen.SetActive(true);
                break;
        }
    }

    public void OpenTitleScreen()
    {
        SetMenu(Menu.TitleScreen);
    }
    public void OpenMainMenu()
    {
        SetMenu(Menu.MainMenu);
    }
    
    public void OpenRoomListMenu()
    {
        SetMenu(Menu.RoomListMenu);
    }
    
    public void OpenInRoomScreen()
    {
        SetMenu(Menu.InRoomScreen);
    }
    
    public void OpenSettingsMenu()
    {
        SetMenu(Menu.SettingsMenu);
    }
    
    public void OpenTutorialScreen()
    {
        SetMenu(Menu.TutorialScreen);
    }
    
    public void OpenDisconnectedScreen()
    {
        SetMenu(Menu.DisconnectedScreen);
    }
    
    public void OpenFeedbackScreen()
    {
        SetMenu(Menu.FeedbackScreen);
    }
    
    public void OpenRulesScreen()
    {
        SetMenu(Menu.RulesScreen);
    }
    
    public void OpenVictoryScreen()
    {
        SetMenu(Menu.VictoryScreen);
    }

    public void  OnClick_ConnectToRoomsBtn()
    {
        if(!NickNameTF.text.Equals("")) //Maybe we should display some UI to notify that the user needs to choose a nickname
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = NickNameTF.text; //sets the nickname of the player
            PhotonNetwork.SendRate = 40;
            PhotonNetwork.SetPlayerCustomProperties(GenerateCustomProperties()); // function used to initialize the skin, maybe we can fetch a local preference
            PhotonNetwork.ConnectUsingSettings(); //uses the API_ID from settings to connect to Photon Server
        }
    }

    private Hashtable GenerateCustomProperties()
    {
        Hashtable _myCustomProperties = new Hashtable();
        _myCustomProperties["SkinID"] = -1;
        return _myCustomProperties;
    }

    public void OnClick_PlayBtn() //go to the main menu
    {
        OpenMainMenu();
    }
    
    public void OnClick_SettingsBtn() //jump to the settings menu
    {
        OpenSettingsMenu();
    }
    
    public void OnClick_TutorialBtn() //jump to the tutorial
    {
        OpenTutorialScreen();
    }

    public void OnClick_DisconnectBtn() // Disconnect from server
    {
        PhotonNetwork.Disconnect();
    }
    
    public void OnClick_ReturnToMain() //now the same for settings/tutorial, maybe we want to save settings or something
    {
        OpenMainMenu();
    }

    public void OnClick_LeaveRoom()
    {
        Debug.Log("Room left");
        PhotonNetwork.LeaveRoom(true);
        OpenRoomListMenu();
    }

    public void OnClick_Quit()
    {
        Application.Quit();
    }

    public override void OnConnectedToMaster() 
    {
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby(TypedLobby.Default); //a mandatory extension to ConnectUsingSettings
    }
    

    public override void OnJoinedLobby() // When the client joins the lobby, enter the room list menu
    {
        OpenRoomListMenu();
    }
    
    public override void OnJoinedRoom()
    {
        OpenInRoomScreen();
        print("JOINED");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("FAILED TO JOIN ROOM");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        OpenDisconnectedScreen();
        Error = cause.ToString();
    }
}
