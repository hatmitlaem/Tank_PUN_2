using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class GameNetwork : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static GameNetwork m_instance;
    [SerializeField]
    private Text m_pingText;
    [SerializeField]
    private ReconnectUI m_reconnectUI;
    [SerializeField]
    private Transform m_playerTimeOutTransform;
    [SerializeField]
    private GameObject m_playerTimeoutPrefabs;
    [SerializeField]
    private Timer m_timer;
    [SerializeField]
    private PhotonView m_PV;
    [SerializeField]
    private Transform m_GruopLeaveText;
    [SerializeField]
    private GameObject m_leaveTextPre;
    List<PlayerTimeOut> m_listPlayerTimeOut = new List<PlayerTimeOut>();
    List<int> m_listPlayerLeave = new List<int>();
    private bool m_allowLeave = false;
    private int m_numPlayerInRoom;
    private bool m_isMaster = false;
    void Awake()
    {
        if (!m_instance)
            m_instance = this;
    }
    void Start()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        if (PhotonNetwork.IsMasterClient)
        {
            m_numPlayerInRoom = PhotonNetwork.PlayerList.Length;
        }
        m_isMaster = PhotonNetwork.IsMasterClient;
        StartCoroutine(getPing());
    }
    void Update()
    {
        //checkNetworkStatus();
    }
    IEnumerator getPing()
    {
        while(PhotonNetwork.IsConnected)
        {
            m_pingText.text ="Ping:"+ PhotonNetwork.GetPing().ToString();
            yield return new WaitForSeconds(1);
        }
        yield break;
    }
    void checkNetworkStatus()
    {
        if (!PhotonNetwork.IsConnected)
        {
            print("Reconnecting...");
            PhotonNetwork.ReconnectAndRejoin();
        }
    }
    void SendStatePlayer()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.Others
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        object[] data = new object[PLayerNetwork.m_instance.m_numPlayerInGame + 1];
        object[] listItem = new object[ItemManager.m_instance.m_listItem.Length + 1];
        listItem[0] = ItemManager.m_instance.m_listItem.Length;
        for (int i = 0; i < ItemManager.m_instance.m_listItem.Length; i++)
        {
            listItem[i + 1] = ItemManager.m_instance.m_listItem[i].activeSelf;
        }
        data[0] = listItem;
        for (int i = 0; i < PLayerNetwork.m_instance.m_numPlayerInGame; i++)
        {
            PlayerManager pm = GameSetup.m_instance.m_listPlayerTeam[i];
            if (!pm)
            {
                Debug.Log("Player " + i + " null");
                data[i + 1] = 0;
                continue;
            }
            Rigidbody rd = pm.gameObject.GetComponent<Rigidbody>();
            TankHealth h = pm.gameObject.GetComponent<TankHealth>();
            bool isDead = h.m_Dead;
            if (isDead)
            {
                object[] content = new object[] {
                    isDead,
                    pm.m_score,
                    rd.position,
                };
                data[i + 1] = content;
            }
            else
            {
                object[] content = new object[] {
                    isDead,
                    pm.m_score,
                    rd.position,
                    rd.velocity,
                    rd.rotation,
                    pm.m_turret.rotation,
                    pm.m_isUpdateShell,
                    h.m_CurrentHealth,
                    h.m_isArmor,
                    h.m_isImmortal,
                };
                data[i + 1] = content;
            }
        }
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.resume, data, raiseEventOptions, sendOptions);
    }
    void CreateLeaveText(string playerName)
    {
        m_numPlayerInRoom--;
        GameObject leaveText = Instantiate(m_leaveTextPre, m_GruopLeaveText);
        Text t = leaveText.GetComponent<Text>();
        t.text = playerName + " has leave game";
    }
    void CreateJoinText(string playerName)
    {
        GameObject leaveText = Instantiate(m_leaveTextPre, m_GruopLeaveText);
        Text t = leaveText.GetComponent<Text>();
        t.text = playerName + " has join game";
    }
    void Pause(bool isConnect = true)
    {
        GameUiManager.m_instance.m_isPause = true;
        m_playerTimeOutTransform.parent.gameObject.SetActive(true);
        if (!isConnect)
        {

        }
    }
    void Resume(float currentTime, float lag = 0)
    {
        m_timer.m_currentTime = currentTime + lag;
        GameUiManager.m_instance.m_isPause = false;
        m_playerTimeOutTransform.parent.gameObject.SetActive(false);
    }
    void ClearListTimeOut()
    {
        for (int i = 0; i < m_listPlayerTimeOut.Count; i++)
            GameObject.Destroy(m_listPlayerTimeOut[i].gameObject);
        m_listPlayerTimeOut.Clear();
    }
    void ClearCache()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.RemoveFromRoomCache,
            Receivers = ReceiverGroup.Others
        };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.resume, 0, raiseEventOptions, sendOptions);

    }
    void DropPlayer(Player player)
    {
        PhotonNetwork.DestroyPlayerObjects(player);
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("Leave", true);
        player.SetCustomProperties(hash);
        m_listPlayerLeave.Add(player.ActorNumber);
        m_PV.RPC("RPC_removeListTimeOut", RpcTarget.All,
                 player.NickName, player.ActorNumber, true);
        if (m_listPlayerTimeOut.Count <= 0)
        {
            float timeSend = (float)PhotonNetwork.Time;
            float currentTime = m_timer.m_currentTime;
            m_PV.RPC("PRC_resume", RpcTarget.All, currentTime, timeSend);
            ClearCache();
        }
    }
    public void DropPlayer(int actorNumber)
    {
        int index = m_listPlayerTimeOut.FindIndex(x => x.m_actorNumber == actorNumber);
        if (index == -1)
            return;
        PlayerTimeOut timeOut = m_listPlayerTimeOut[index];
        timeOut.m_numDrop++;
        if (timeOut.m_numDrop >= m_numPlayerInRoom - m_listPlayerTimeOut.Count)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i].ActorNumber == actorNumber)
                {
                    DropPlayer(PhotonNetwork.PlayerList[i]);
                    return;
                }
            }
        }

    }
    public void OnClickLeaveMatch(bool isClick = true)
    {
        if (PhotonNetwork.IsConnected)
        {
           ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("Leave", true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            if (isClick)
                PhotonNetwork.LeaveRoom(false);
            else
            {
                PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                PhotonNetwork.LeaveRoom();
            }
            m_allowLeave = true;
        }
        else
            PhotonNetwork.LoadLevel(0);

    }
    /// ///////////////////////
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == (byte)PhotonEventCodes.resume)
        {
            object[] data = (object[])photonEvent.CustomData;
            object[] listItemObj = (object[])data[0];
            int length = (int)listItemObj[0];
            bool[] listItem = new bool[length];
            for (int i = 0; i < length; i++)
            {
                listItem[i] = (bool)listItemObj[i + 1];
            }
            ItemManager.m_instance.SetItem(listItem);
            {
                for (int i = 0; i < PLayerNetwork.m_instance.m_numPlayerInGame; i++)
                {
                    PlayerManager pm = GameSetup.m_instance.m_listPlayerTeam[i];
                    if (!pm)
                    {
                        Debug.Log("Player " + i + " null");
                        continue;
                    }
                    Rigidbody rd = pm.gameObject.GetComponent<Rigidbody>();
                    TankHealth h = pm.gameObject.GetComponent<TankHealth>();
                    object[] content = (object[])data[i + 1];
                    bool isDead = (bool)content[0];
                    int score = (int)content[1];
                    Vector3 pos = (Vector3)content[2];
                    h.SetDead(isDead);
                    pm.SetScore(score);
                    rd.position = pos;
                    if (!isDead)
                    {
                        Vector3 velocity = (Vector3)content[3];
                        Quaternion rotation = (Quaternion)content[4];
                        Quaternion turretRotation = (Quaternion)content[5];
                        bool shellUpdate = (bool)content[6];
                        float healthUpdate = (float)content[7];
                        bool armorUpdate = (bool)content[8];
                        bool immortalUpdate = (bool)content[9];
                        rd.velocity = velocity;
                        rd.rotation = rotation;
                        pm.m_turret.rotation = turretRotation * Quaternion.Inverse(rotation);
                        pm.SetScore(score);
                        pm.SetShell(shellUpdate);
                        h.SetHealth(healthUpdate);
                        h.SetArmor(armorUpdate);
                        h.SetImmortal(immortalUpdate);
                    }
                }
            }
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        bool leave = (bool)otherPlayer.CustomProperties["Leave"];
        if (leave)
        {
            if (m_listPlayerLeave.FindIndex(x => x == otherPlayer.ActorNumber) == -1)
            {
                CreateLeaveText(otherPlayer.NickName);
                m_listPlayerLeave.Add(otherPlayer.ActorNumber);
            }
        }
        else
        {
            if (m_isMaster)
            {
                int index = m_listPlayerTimeOut.FindIndex(x => x.m_actorNumber == otherPlayer.ActorNumber);
                if (index == -1)
                {
                    if (!GameUiManager.m_instance.m_isPause)
                        SendStatePlayer();
                    float timeSend = (float)PhotonNetwork.Time;
                    m_PV.RPC("RPC_addListTimeOut", RpcTarget.All,
                        otherPlayer.NickName, otherPlayer.ActorNumber, timeSend);
                }
                else
                {
                    m_PV.RPC("RPC_removeListTimeOut", RpcTarget.All,
                        otherPlayer.NickName, otherPlayer.ActorNumber, true);
                    if (m_listPlayerTimeOut.Count <= 0)
                    {
                        float timeSend = (float)PhotonNetwork.Time;
                        float currentTime = m_timer.m_currentTime;
                        m_PV.RPC("PRC_resume", RpcTarget.All, currentTime, timeSend);
                        ClearCache();
                    }
                }
            }
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!m_isMaster)
            return;
        m_numPlayerInRoom = PhotonNetwork.PlayerList.Length;
        int index = m_listPlayerTimeOut.FindIndex(x => x.m_actorNumber == newPlayer.ActorNumber);
        if (index != -1)
        {
            m_PV.RPC("RPC_removeListTimeOut", RpcTarget.All,
                newPlayer.NickName, newPlayer.ActorNumber, false);
            if (m_listPlayerTimeOut.Count <= 0)
            {
                float timeSend = (float)PhotonNetwork.Time;
                float currentTime = m_timer.m_currentTime;
                m_PV.RPC("PRC_resume", RpcTarget.All, currentTime, timeSend);
                ClearCache();
            }
            else
            {
                float timeSend = (float)PhotonNetwork.Time;
                int length = m_listPlayerTimeOut.Count;
                int[] listActor = new int[length];
                string[] listName = new string[length];
                float[] listTime = new float[length];
                for (int i = 0; i < length; i++)
                {
                    PlayerTimeOut timeOut = m_listPlayerTimeOut[i];
                    listActor[i] = timeOut.m_actorNumber;
                    listName[i] = timeOut.GetName();
                    listTime[i] = timeOut.GetCurrentTime();
                }
                m_PV.RPC("RPC_updateListTimeOut", RpcTarget.Others,
                    newPlayer.ActorNumber, timeSend, length, listActor, listName, listTime);
            }
        }

    }
    public override void OnLeftRoom()
    {
        if (!m_allowLeave)
            return;
        PhotonNetwork.LoadLevel(0);
        Destroy(GameObject.Find("DDOL"));
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!GameUiManager.m_instance.m_isGameOver)
            OnClickLeaveMatch(false);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        print("OnDisconnected cause: " + cause);
        ClearListTimeOut();
        Pause();
        m_reconnectUI.SetActive(true, m_isMaster);
    }
    public override void OnConnected()
    {
        if (!m_isMaster)
            m_reconnectUI.SetActive(false);
        StartCoroutine(getPing());
    }
    [PunRPC]
    void RPC_addListTimeOut(string namePlayer, int actorNumber, float timeSend)
    {
        float lag = (float)PhotonNetwork.Time - timeSend;
        GameObject timOutObj = Instantiate(m_playerTimeoutPrefabs, m_playerTimeOutTransform);
        PlayerTimeOut timeOut = timOutObj.GetComponent<PlayerTimeOut>();
        timeOut.SetPlayer(namePlayer, actorNumber);
        timeOut.SetPauseTimer(true, lag);
        m_listPlayerTimeOut.Add(timeOut);
        Pause();
    }
    [PunRPC]
    void RPC_removeListTimeOut(string namePlayer, int actorNumber, bool isLeft)
    {
        if (isLeft)
            CreateLeaveText(namePlayer);
        else
            CreateJoinText(namePlayer);
        int index = m_listPlayerTimeOut.FindIndex(x => x.m_actorNumber == actorNumber);
        if (index == -1)
            return;
        PlayerTimeOut timeOut = m_listPlayerTimeOut[index];
        m_listPlayerTimeOut.RemoveAt(index);
        GameObject.Destroy(timeOut.gameObject);
    }
    [PunRPC]
    void RPC_updateListTimeOut(int actorNumber, float timeSend, int length, int[] listActorNumber, string[] listName, float[] listTime)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != actorNumber)
            return;
        float lag = (float)PhotonNetwork.Time - timeSend;
        for (int i = 0; i < length; i++)
        {
            GameObject timOutObj = Instantiate(m_playerTimeoutPrefabs, m_playerTimeOutTransform);
            PlayerTimeOut timeOut = timOutObj.GetComponent<PlayerTimeOut>();
            timeOut.SetPlayer(listName[i], listActorNumber[i]);
            timeOut.SetPauseTimer(true, lag + listTime[i]);
            m_listPlayerTimeOut.Add(timeOut);
        }
        Pause();
    }
    [PunRPC]
    void PRC_resume(float currentTime, float sendTime)
    {
        float lag = (float)PhotonNetwork.Time - sendTime;
        Resume(currentTime, lag);
    }
}
