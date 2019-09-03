using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
public class CreateRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private RoomLayoutGroup m_roomLayoutGroup;
    [SerializeField]
    private GameObject m_FailedNotificationObj;
    [SerializeField]
    private Text m_m_FailedNotificationText;
    private string m_roomName="";
    public void OnClick_CreateRoom()
    {
        string[] LobbyOptions = new string[1];
        LobbyOptions[0] = "NumKill";
        Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() {
        { LobbyOptions[0], RoomSetting.m_instance.m_numKill }
    };
        RoomOptions roomOps = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 4,
            CustomRoomPropertiesForLobby = LobbyOptions,
            CustomRoomProperties = customProperties,
            PlayerTtl = 30000,
        };

        if (m_roomName.Equals(""))
            m_roomName = PLayerNetwork.m_instance.m_name;
        if (PhotonNetwork.CreateRoom(m_roomName, roomOps,TypedLobby.Default))
        {
            print("create room successfully sent");
        }
        else
        {
            print("create room failed sent");
        }
    }

    public void SetRoomName(Text name)
    {
        m_roomName = name.text;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("create room failed: " + message);
        m_FailedNotificationObj.SetActive(true);
        m_m_FailedNotificationText.text = message;
    }

    public override void OnCreatedRoom()
    {
        if (m_roomLayoutGroup)
            m_roomLayoutGroup.RemoveAllRoom();
        print("create room successfully");
    }

}
