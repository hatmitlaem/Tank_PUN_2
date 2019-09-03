using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class LobbyNetwork : MonoBehaviourPunCallbacks
{
    public GameObject m_loadObj;
    public GameObject m_disConnectObj;
    public InputField m_nameTextField;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            print("Connecting to sever...");
            PhotonNetwork.ConnectUsingSettings();
            m_loadObj.SetActive(true);
        }
        m_nameTextField.text = PLayerNetwork.m_instance.m_name;
    }

    void Update()
    {
        checkNetworkStatus();

    }

    public override void OnConnectedToMaster()
    {
        print("Connected to master");
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.NickName = PLayerNetwork.m_instance.m_name;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        m_loadObj.SetActive(false);
        print("JoinedLobb");
    }
    public override void OnConnected()
    {
        m_disConnectObj.SetActive(false);
        MainCanvasManager.m_instance.m_lobbyCanvas.Clear();
        MainCanvasManager.m_instance.m_currentRoomCanvas.Clear();
        print("OnConnected");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        m_disConnectObj.SetActive(true);
        m_loadObj.SetActive(false);
        print("OnDisconnected cause: " + cause);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        MainCanvasManager.m_instance.m_currentRoomCanvas.Clear();
    }

    void checkNetworkStatus()
    {
        if (!PhotonNetwork.IsConnected)
        {
            print("Reconnecting...");
            PhotonNetwork.Reconnect();
        }
    }

}
