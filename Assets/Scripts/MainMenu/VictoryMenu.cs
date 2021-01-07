using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class VictoryMenu : MonoBehaviour
{
    
    [SerializeField] private Image _winnerImage;
    [SerializeField] private Text _winnerText;
    [SerializeField] private Sprite[] catWinningPortraitList;
    [SerializeField] private Sprite[] catLosingPortraitList;
    [SerializeField] private Material[] catMaterialList;
    [SerializeField] private ScreenManager screenManager;
    [SerializeField] private GameObject loserPrefab;
    [SerializeField] private Transform content;
    
    void OnEnable()
    {
        Player winner = CrossSceneVictoryInfo.Winner;
        if (winner != null)
        {
            int _skin = (int) winner.CustomProperties["SkinID"];
            CatSkin targetSkin = CatSkins.catSkinsList[_skin];
            _winnerImage.sprite = catWinningPortraitList[targetSkin.baseSkinID];
            Material mat = new Material(catMaterialList[targetSkin.baseSkinID]);
            mat.SetColor("_SkinABC",
                new Color(targetSkin.skinColor.r / 255, targetSkin.skinColor.g / 255, targetSkin.skinColor.b / 255));
            mat.SetColor("_DotsABC",
                new Color(targetSkin.dotsColor.r / 255, targetSkin.dotsColor.g / 255, targetSkin.dotsColor.b / 255));
            mat.SetColor("_DetailsABC",
                new Color(targetSkin.detailsColor.r / 255, targetSkin.detailsColor.g / 255,
                    targetSkin.detailsColor.b / 255));
            _winnerImage.material = mat;
            _winnerText.text = winner.NickName + " Wins!!";
            
            foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
                if(!playerInfo.Value.Equals(winner))
                    AddLoserListing(playerInfo.Value);
        }
    }

    public void OnClick_ReturnToRoomBtn()
    {
        screenManager.OpenInRoomScreen();
    }

    private void AddLoserListing(Player player)
    {
        GameObject listing = Instantiate(loserPrefab, content);
        int _skin = (int) player.CustomProperties["SkinID"];
        CatSkin targetSkin = CatSkins.catSkinsList[_skin];
        listing.GetComponentInChildren<Image>().sprite = catLosingPortraitList[targetSkin.baseSkinID];
        Material mat = new Material(catMaterialList[targetSkin.baseSkinID]);
        mat.SetColor("_SkinABC",
            new Color(targetSkin.skinColor.r / 255, targetSkin.skinColor.g / 255, targetSkin.skinColor.b / 255));
        mat.SetColor("_DotsABC",
            new Color(targetSkin.dotsColor.r / 255, targetSkin.dotsColor.g / 255, targetSkin.dotsColor.b / 255));
        mat.SetColor("_DetailsABC",
            new Color(targetSkin.detailsColor.r / 255, targetSkin.detailsColor.g / 255,
                targetSkin.detailsColor.b / 255));
        listing.GetComponentInChildren<Image>().material = mat;
        listing.GetComponentInChildren<Text>().text = player.NickName;
    }
    
    public void OnDisable()
    {
        content.DestroyChildren();
    }
}
