using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerListing _playerListing; //The prefab of a listing
    [SerializeField] private Transform _content; //where the listings will be spawned 
    [SerializeField] private GameObject _ReadyBtn;
    private bool _isReady = false;
    
    private List<PlayerListing> _listings = new List<PlayerListing>(); //Structure to keep the room list

    public override void OnEnable()
    {
        base.OnEnable();
        SetReady(false);
        GetCurrentRoomPlayers();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        _listings.Clear();
        _content.DestroyChildren();
    }

    private void GetCurrentRoomPlayers()
    {
        if (!PhotonNetwork.IsConnected) return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null) return;
        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
            AddPlayerListing(playerInfo.Value);
    }

    private void AddPlayerListing(Player player)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            _listings[index].SetPlayerInfo(player);
        }
        else
        {
            PlayerListing listing = Instantiate(_playerListing, _content); //if a players enter, its listing is created
            if (listing != null)
                listing.SetPlayerInfo(player);
            _listings.Add(listing);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }
    
    public void OnClick_ReadyBtn()
    {
        SetReady(!_isReady);
        photonView.RPC("OnChangeReadyState_RPC", RpcTarget.All, PhotonNetwork.LocalPlayer, _isReady);
    }

    [PunRPC]
    public void OnChangeReadyState_RPC(Player player, bool ready)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            _listings[index].SetReady(ready);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            bool startGame = true;
            foreach (PlayerListing p in _listings)
                if (!p.IsReady)
                    startGame = false;
            if (startGame)
                StartGame();
                
        }
    }

    private void SetReady(bool ready)
    {
        _isReady = ready;
        if (ready)
        {
            _ReadyBtn.GetComponentInChildren<Text>().text = "Not Ready";
            _ReadyBtn.GetComponentInChildren<Text>().color = Color.red;
        }
        else
        {
            _ReadyBtn.GetComponentInChildren<Text>().text = "Ready";
            _ReadyBtn.GetComponentInChildren<Text>().color = Color.magenta;
        }
    }
    
    private void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
    }
}
