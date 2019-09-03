using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;
public class PlayerLauoutGroup : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject m_playerListingPrefabs;
    [SerializeField]
    private GameObject m_startObj;
    [SerializeField]
    private Text m_roomNameText;
    [SerializeField]
    private Text m_numKillText;
    private List<PlayerListing> m_playerListings = new List<PlayerListing>();

    public override void OnJoinedRoom()
    {
        if (m_startObj)
        {
            if (PhotonNetwork.IsMasterClient)
                m_startObj.SetActive(true);
            else
                m_startObj.SetActive(false);
        }
        MainCanvasManager.m_instance.m_lobbyCanvas.Clear();
        MainCanvasManager.m_instance.m_lobbyCanvas.gameObject.transform.SetAsFirstSibling();
        MainCanvasManager.m_instance.m_lobbyCanvas.Hide();

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            AddPlayer(PhotonNetwork.PlayerList[i]);
        }
        m_roomNameText.text ="Room Name:"+ PhotonNetwork.CurrentRoom.Name;
        m_numKillText.text = PhotonNetwork.CurrentRoom.CustomProperties["NumKill"].ToString();
        Hashtable hash = new Hashtable();
        hash.Add("Leave", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            AddPlayer(PhotonNetwork.PlayerList[i]);
        }
    }

    //luu y: co the gay ra bug dong bo khi nhieu player left room cung luc
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayer(otherPlayer);
    }
    void AddPlayer(Player newPlayer)
    {
        if (newPlayer == null)
            return;
        int index = m_playerListings.FindIndex(x => x.m_photonPlayer == newPlayer);
        if (index != -1)
            return;
        GameObject playerListingObj = Instantiate(m_playerListingPrefabs);
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(newPlayer);
        m_playerListings.Add(playerListing);

        if (newPlayer.IsMasterClient)
            playerListing.m_keyIcon.SetActive(true);
        else
            playerListing.m_keyIcon.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            if (newPlayer.IsMasterClient)
                playerListing.m_kickButton.SetActive(false);
            else
            {
                playerListing.m_kickButton.SetActive(true);
                playerListing.m_kickButton.GetComponent<Button>().onClick.AddListener(() => MainCanvasManager.m_instance.m_currentRoomCanvas.OnClickKickPlayer(playerListing));
            }
        }
        else
        {
            playerListing.m_kickButton.SetActive(false);
        }
    }

    void RemovePlayer(Player player)
    {
        int index = m_playerListings.FindIndex(x => x.m_photonPlayer == player);
        if (index != -1)
        {
            GameObject.Destroy(m_playerListings[index].gameObject);
            m_playerListings.RemoveAt(index);
        }
    }

    public void RemoveAll()
    {
        for (int i = 0; i < m_playerListings.Count; i++)
        {
            GameObject.Destroy(m_playerListings[i].gameObject);
        }
        m_playerListings.Clear();
    }


}
