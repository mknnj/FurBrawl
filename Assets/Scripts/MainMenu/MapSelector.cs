using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MapSelector : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject nextMapBtn;
    [SerializeField] private GameObject previousMapBtn;
    [SerializeField] private int mapID;
    [SerializeField] private Sprite[] mapLayoutsList;
    [SerializeField] private string[] mapNamesList;
    [SerializeField] private Text mapName;
    [SerializeField] private Image mapLayout; 
    
    public override void OnEnable()
    {
        base.OnEnable();
        Debug.Log("Master: "+PhotonNetwork.MasterClient.NickName); //If error here, start by enabling another screen
        if (!PhotonNetwork.IsMasterClient)
        {
            nextMapBtn.SetActive(false);
            previousMapBtn.SetActive(false);
        }
        SetMapID(0);
    }

    public void OnClick_NextMapBtn()
    {
        mapID++;
        if (mapID >= mapLayoutsList.Length)
            mapID = 0;
        photonView.RPC("SyncMapIdRPC", RpcTarget.AllViaServer, mapID);
    }
    
    public void OnClick_PrevMapBtn()
    {
        Debug.Log("Clicked prev");
        mapID--;
        if (mapID < 0)
            mapID = mapLayoutsList.Length-1;
        photonView.RPC("SyncMapIdRPC", RpcTarget.AllViaServer, mapID);
    }

    [PunRPC]
    public void SyncMapIdRPC(int newMapID)
    {
        SetMapID(newMapID);
    }

    public void SetMapID(int id)
    {
        mapID = id;
        mapLayout.sprite = mapLayoutsList[mapID];
        mapName.text = mapNamesList[mapID];
        
        CrossSceneMapInfo.SetMapID(mapID);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            nextMapBtn.SetActive(false);
            previousMapBtn.SetActive(false);
        }
        else
        {
            nextMapBtn.SetActive(true);
            previousMapBtn.SetActive(true); 
        }
    }
}
