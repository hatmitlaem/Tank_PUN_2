using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
public class CurrentRoomCanvas : MonoBehaviourPun, IOnEventCallback
{
    [SerializeField]
    PlayerLauoutGroup m_group;  
    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode ==(byte)PhotonEventCodes.kickPlayer)
        {
            object[] data = (object[])photonEvent.CustomData;
            int num = (int)data[0];
            if(num == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                LeaveRoom();
            }
        }
    }
    public void OnClickLeaveRoom()
    {
        LeaveRoom();
    }

    public void OnClickStart()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(false);
    }

    public void OnClickKickPlayer(PlayerListing playerlisting)
    {
        KickPlayer(playerlisting.m_photonPlayer);
    }
    void KickPlayer(Player playerKick)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.Others
        }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        SendOptions sendOptions = new SendOptions { Reliability = true};

        object[] content = new object[] { playerKick.ActorNumber };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.kickPlayer, content, raiseEventOptions, sendOptions);

    }

    public void Clear()
    {
        MainCanvasManager.m_instance.m_lobbyCanvas.Show();
        MainCanvasManager.m_instance.m_currentRoomCanvas.gameObject.transform.SetAsFirstSibling();
        if (m_group)
            m_group.RemoveAll();
    }

}
