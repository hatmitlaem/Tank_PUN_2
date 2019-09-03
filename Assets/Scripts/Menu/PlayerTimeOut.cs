using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class PlayerTimeOut : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField]
    GameObject button;
    [SerializeField]
    private Text m_nameText;
    [SerializeField]
    pauseTimer m_pauseTimer;

    [HideInInspector]
    public int m_actorNumber = -1;
    [HideInInspector]
    public int m_numDrop = 0;

    public void SetPlayer(string name,int actorNumber)
    {
        m_nameText.text = name;
        m_actorNumber = actorNumber;
    }

    public void SetPauseTimer(bool flag, float lag =0, float time = 30)
    {
        m_pauseTimer.SetActive(flag, lag, time);
    }
    public float GetCurrentTime()
    {
        return m_pauseTimer.m_currentTime;
    }
    public string GetName()
    {
        return m_nameText.text;
    }

    public void OnClickDropPlayer()
    {
        DropPlayer();
        button.SetActive(false);
    }

    void DropPlayer()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.MasterClient
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        object[] content = new object[] { m_actorNumber };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.dropPlayer, content, raiseEventOptions, sendOptions);
    }
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == (byte)PhotonEventCodes.dropPlayer)
        {
            object[] data = (object[])photonEvent.CustomData;
            int actorNumber = (int)data[0];
            if (actorNumber == m_actorNumber)
                GameNetwork.m_instance.DropPlayer(actorNumber);
        }
    }
}
