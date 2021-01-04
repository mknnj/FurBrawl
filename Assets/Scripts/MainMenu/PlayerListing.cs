using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
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
    
    [SerializeField] private TextMeshProUGUI _SkinIdText; // for debugging purpose, or we can explicit the type of cat

    [SerializeField] private Sprite[] catPortraitsList;
    [SerializeField] private Material[] catMaterialList;
    private PlayerListingMenu parentMenu;
    public Player Player { get; private set; }
    public bool IsReady {get; private set; }//tells if the player is ready or not

    //To be called immediately when the listing is instantiated to set the parent
    public void SetParentMenu(PlayerListingMenu parentMenu)
    {
        this.parentMenu = parentMenu;
    }
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
    
    //when you click left/right button, the client searches for the next available skin in that direction
    public void OnClick_LeftSkinBtn()
    {
        var newCustomProperties = new Hashtable();
        newCustomProperties["SkinID"] =
            parentMenu.GetNextAvailableSkin((int)PhotonNetwork.LocalPlayer.CustomProperties["SkinID"],-1);
        PhotonNetwork.SetPlayerCustomProperties(newCustomProperties) ;
       
    }
    
    public void OnClick_RightSkinBtn()
    {
        var newCustomProperties = new Hashtable();
        newCustomProperties["SkinID"] = 
            parentMenu.GetNextAvailableSkin((int)PhotonNetwork.LocalPlayer.CustomProperties["SkinID"],1);
        PhotonNetwork.SetPlayerCustomProperties(newCustomProperties);
    }

    

    //Called whenever there is an update on custom player properties( ready/skin selection), it keeps updated the info stored on the client
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

    //Used to visualize custom player property on each player's screen
    private void SetPlayerProperties(Player player)
    {
        _name.text = player.NickName;
        _skin = (int)player.CustomProperties["SkinID"];
        if (_skin >= 0)
        {
            CatSkin targetSkin = CatSkins.catSkinsList[_skin];

            _image.sprite = catPortraitsList[targetSkin.baseSkinID];
            Material mat = new Material(catMaterialList[targetSkin.baseSkinID]);
            mat.SetColor("_SkinABC",
                new Color(targetSkin.skinColor.r / 255, targetSkin.skinColor.g / 255, targetSkin.skinColor.b / 255));
            mat.SetColor("_DotsABC",
                new Color(targetSkin.dotsColor.r / 255, targetSkin.dotsColor.g / 255, targetSkin.dotsColor.b / 255));
            mat.SetColor("_DetailsABC",
                new Color(targetSkin.detailsColor.r / 255, targetSkin.detailsColor.g / 255,
                    targetSkin.detailsColor.b / 255));
            _image.material = mat;
            _SkinIdText.text = "<mark=#8cc3cf55 padding=\"10, 10, 10, 10\">"+targetSkin.name+"</mark>";
        }
    }
    
    public void SetReady(bool ready)
    {
        IsReady = ready;
        _isReadyText.enabled = ready;
        //Hide arrows for skin selection if the player is ready
        if (Player.Equals(PhotonNetwork.LocalPlayer))
        {
            _leftSkinBtn.SetActive(!IsReady);
            _rightSkinBtn.SetActive(!IsReady);
        }
    }
    
    
}
