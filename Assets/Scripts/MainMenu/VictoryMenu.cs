using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class VictoryMenu : MonoBehaviour
{
    
    [SerializeField] private Image _winnerImage;
    [SerializeField] private Text _winnerText;
    [SerializeField] private Sprite[] catSkinsList;
    [SerializeField] private Material[] catMaterialList;
    [SerializeField] private ScreenManager screenManager;
    void OnEnable()
    {
        
        Player winner = CrossSceneVictoryInfo.Winner;
        if (winner != null)
        {
            int _skin = (int) winner.CustomProperties["SkinID"];
            CatSkin targetSkin = CatSkins.catSkinsList[_skin];
            _winnerImage.sprite = catSkinsList[targetSkin.baseSkinID];
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
        }
    }

    public void OnClick_ReturnToRoomBtn()
    {
        screenManager.OpenInRoomScreen();
    }
}
