using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class LobbyCanvas : MonoBehaviour
{
    [SerializeField]
    private RoomLayoutGroup m_roomLayoutGroup;
    [SerializeField]
    private GameObject m_ButtonObj;
    [SerializeField]
    private GameObject m_RoomInfo;
   
    public void OnClickJoinRoom(string roomName)
    {
        if(PhotonNetwork.JoinRoom(roomName))
        {
        }
        else
        {
            print("join room failed");
        }
    }

    public void Show()
    {
        m_ButtonObj.SetActive(true);
        m_RoomInfo.SetActive(false);
    }
    public void Hide()
    {
        m_ButtonObj.SetActive(false);
        m_RoomInfo.SetActive(true);
    }
    public void Clear()
    {
        m_roomLayoutGroup.RemoveAllRoom();
    }
}
