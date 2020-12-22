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
    [SerializeField] private ScreenManager screenManager;
    void OnEnable()
    {
        
        Player winner = CrossSceneVictoryInfo.Winner;
        if (winner != null)
        {
            int _skin = (int) winner.CustomProperties["SkinID"];

            _winnerImage.sprite = catSkinsList[_skin];
            _winnerText.text = winner.NickName + " Wins!!";
        }
    }

    public void OnClick_ReturnToRoomBtn()
    {
        screenManager.OpenInRoomScreen();
    }
}
