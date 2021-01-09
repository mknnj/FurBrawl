using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MapSelector : MonoBehaviourPunCallbacks
{
    [SerializeField] private int mapID;
    [SerializeField] private Sprite[] mapLayoutsList;
    [SerializeField] private RectTransform content;
    [SerializeField] private MapIcon iconPrefab;
    [SerializeField] private List<MapIcon> mapIcons = new List<MapIcon>(); 
    
    public override void OnEnable()
    {
        base.OnEnable();
        Debug.Log("Master: "+PhotonNetwork.MasterClient.NickName); //If error here, start by enabling another screen
        mapIcons.Clear();
        content.DestroyChildren();
        for (int i =0 ; i< mapLayoutsList.Length; i++ )
        {
            var icon = Instantiate(iconPrefab, content);
            icon.SetData(mapLayoutsList[i], this, i);
            mapIcons.Add(icon);
        }
        SetMapID(0);
        if (!PhotonNetwork.IsMasterClient)
        {
            foreach (var icon in mapIcons)
                icon.GetComponent<Button>().enabled = false;
            photonView.RPC("RequestSyncIdRPC", RpcTarget.MasterClient);
        }
    }
    
    public void Clicked(int id)
    {
        mapID = id;
        photonView.RPC("SyncMapIdRPC", RpcTarget.AllViaServer, mapID);
        CrossSceneMapInfo.SetMapID(mapID);
    }

    [PunRPC]
    public void SyncMapIdRPC(int newMapID)
    {
        SetMapID(newMapID);
    }
    
    [PunRPC]
    public void RequestSyncIdRPC()
    {
        if(PhotonNetwork.IsMasterClient)
            photonView.RPC("SyncMapIdRPC", RpcTarget.AllViaServer, mapID);
    }

    public void SetMapID(int id)
    {
        mapID = id;
        foreach (var i in mapIcons)
            if (i.mapID != id)
                i.Deselect();
            else
                i.Select();
        CrossSceneMapInfo.SetMapID(mapID);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            foreach (var icon in mapIcons)
                icon.GetComponent<Button>().enabled = false;
        }
        else
        {
            foreach (var icon in mapIcons)
                icon.GetComponent<Button>().enabled = true;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncMapIdRPC", RpcTarget.AllViaServer, mapID);
        }
    }
}
