using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerListing : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text _name; //reference to the text where to write player name
    [SerializeField] private int _skin; //skin the player has
    [SerializeField] private Image _image; //reference to the image to show the skin
    
    [SerializeField] private Text _isReadyText; //used to show if a player is ready or not
    [SerializeField] private GameObject _leftSkinBtn; // references to btn to hide them if the player is not the local one
    [SerializeField] private GameObject _rightSkinBtn;
    [SerializeField] private Text _SkinIdText; // for debugging purpose, or we can explicit the type of cat

    [SerializeField] private Sprite[] catSkinsList;
    public Player Player { get; private set; }
    public bool IsReady {get; private set; }//tells if the player is ready or not
    
    public void SetPlayerInfo(Player player)
    {
        Player = player;
        
        IsReady = false;
        _isReadyText.enabled = false;
        
        SetPlayerProperties(player);
        
        if (!player.Equals(PhotonNetwork.LocalPlayer))
        {
            _leftSkinBtn.SetActive(false);
            _rightSkinBtn.SetActive(false);
        }
    }
    
    public void OnClick_LeftSkinBtn()
    {
        var newCustomProperties = new Hashtable();
        newCustomProperties["SkinID"] = (int) PhotonNetwork.LocalPlayer.CustomProperties["SkinID"] - 1;
        if ((int)newCustomProperties["SkinID"] < 0)
            newCustomProperties["SkinID"] = catSkinsList.Length - 1;
        PhotonNetwork.SetPlayerCustomProperties(newCustomProperties) ;
       
    }
    
    public void OnClick_RightSkinBtn()
    {
        var newCustomProperties = new Hashtable();
        newCustomProperties["SkinID"] = (int) PhotonNetwork.LocalPlayer.CustomProperties["SkinID"] + 1;
        if ((int)newCustomProperties["SkinID"] > catSkinsList.Length - 1)
            newCustomProperties["SkinID"] = 0;
        PhotonNetwork.SetPlayerCustomProperties(newCustomProperties) ;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer != null && targetPlayer == Player)
        {
            if (changedProps.ContainsKey("SkinID"))
            {
                SetPlayerProperties(targetPlayer);
            }
        }
    }

    private void SetPlayerProperties(Player player)
    {
        _name.text = player.NickName;
        _skin = (int)player.CustomProperties["SkinID"];

        _image.sprite = catSkinsList[_skin];
        _SkinIdText.text = _skin.ToString();
    }

    public void SetReady(bool ready)
    {
        IsReady = ready;
        _isReadyText.enabled = ready;
    }
}
