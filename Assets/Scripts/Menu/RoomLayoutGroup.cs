using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class RoomLayoutGroup : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [SerializeField]
    private GameObject m_roomListingPrefabs;

    private List<RoomListing> m_roomListingButtons = new List<RoomListing>();
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        RemoveOldRooms(PhotonNetwork.CurrentRoom.Name);
        MainCanvasManager.m_instance.m_currentRoomCanvas.LeaveRoom();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            RoomReceived(room);
        }
        RemoveOldRooms();
    }

    void RoomReceived(RoomInfo room)
    {
        int index = m_roomListingButtons.FindIndex(x => x.m_roomName == room.Name);
        if (index == -1)
        {
            if (room.IsVisible && room.PlayerCount < room.MaxPlayers && room.PlayerCount > 0)
            {
                GameObject roomListingObj = Instantiate(m_roomListingPrefabs);
                roomListingObj.transform.SetParent(transform, false);

                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                m_roomListingButtons.Add(roomListing);

                index = (m_roomListingButtons.Count - 1);
            }
        }
        else
        {
            if(!(room.IsVisible && room.PlayerCount < room.MaxPlayers && room.PlayerCount > 0))
            {
                RoomListing roomListing = m_roomListingButtons[index];
                roomListing.m_updated = false;
            }
        }

        if (index != -1 && room.PlayerCount > 0)
        {
            RoomListing roomListing = m_roomListingButtons[index];
            roomListing.SetRoomNameText(room.Name);
            roomListing.SetNumPlayerText(room.PlayerCount.ToString() + "/" + room.MaxPlayers.ToString());
            roomListing.SetNumKillText(room.CustomProperties["NumKill"].ToString());
            roomListing.m_updated = true;
        }
        
    }

    void RemoveOldRooms()
    {
        List<RoomListing> removeRooms = new List<RoomListing>();
        foreach (RoomListing room in m_roomListingButtons)
        {
            if (!room.m_updated)
                removeRooms.Add(room);
        }

        foreach (RoomListing room in removeRooms)
        {
            GameObject roomListingObj = room.gameObject;
            m_roomListingButtons.Remove(room);
            GameObject.Destroy(roomListingObj);
        }

    }

    void RemoveOldRooms(string roomName)
    {
        int index = m_roomListingButtons.FindIndex(x => x.m_roomName == roomName);
        if (index == -1)
            return;
        GameObject roomListingObj = m_roomListingButtons[index].gameObject;
        m_roomListingButtons.RemoveAt(index);
        GameObject.Destroy(roomListingObj);

    }

    public void RemoveAllRoom()
    {
        for (int i = 0; i < m_roomListingButtons.Count; i++)
        {
            GameObject roomListingObj = m_roomListingButtons[i].gameObject;
            GameObject.Destroy(roomListingObj);
        }
        m_roomListingButtons.Clear();
    }
}
