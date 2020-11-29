using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private RoomListing _roomListing; //The prefab of a listing
    [SerializeField] private Transform _content; //where the listings will be spawned 

    private List<RoomListing> _listings = new List<RoomListing>();
    // BUG: if 2 clients enter the same room and then both leave, if a client reconnect, sees again the room with 1 player event it should be deleted

    public override void OnJoinedRoom()
    {
        Debug.Log("cleared room");
        _listings.Clear();
        _content.DestroyChildren(); //when a room is joined, the list of rooms should be deleted
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) //Used to keep updated the list of rooms
    {
        Debug.Log("update on room list");
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if (index != -1)
                {
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                }
            }
            else
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if (index == -1)
                {
                    RoomListing listing = Instantiate(_roomListing, _content); //if a room is created, we need to instantiate a listing
                    if (listing != null)
                    {
                        listing.SetRoomInfo(info);
                        _listings.Add(listing);
                    }
                }
                else
                {
                    _listings[index].SetRoomInfo(info);
                }
            }
        }
    }

    
}
