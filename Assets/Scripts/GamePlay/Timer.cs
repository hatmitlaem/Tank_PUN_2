
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class Timer : MonoBehaviourPun
{
    [SerializeField]
    private Text m_text;
    [SerializeField]
    private PhotonView m_PV;
    [HideInInspector]
    public float m_currentTime = 0;
    private bool m_flag;
    void Start()
    {
        m_flag = true;
        if (PhotonNetwork.IsMasterClient)
        {
            float time = (float)PhotonNetwork.Time;
            m_PV.RPC("RPC_SyncTime", RpcTarget.Others, time);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameUiManager.m_instance.m_isPause)
            return;
        m_currentTime += Time.deltaTime;
        int time = (int)m_currentTime;
        int min = time / 60;
        int sec = time % 60;
        SetTimeText(min, sec);
        SetUpItem(sec);
    }

    [PunRPC]
    void RPC_SyncTime(float time)
    {
        float lag = (float)PhotonNetwork.Time - time;
        m_currentTime += lag;
    }
    void SetTimeText(int min, int sec)
    {

        string text = "";
        if (min < 10)
            text += "0";
        text += min.ToString() + ".";
        if (sec < 10)
            text += "0";
        text += sec.ToString();
        m_text.text = text;
    }

    void SetUpItem(int sec)
    {
        if(sec <30)
        {
            if(!m_flag)
            {
                m_flag = !m_flag;
                ItemManager.m_instance.SetItem(m_flag);
            }
        }
        else
        {
            if (m_flag)
            {
                m_flag = !m_flag;
                ItemManager.m_instance.SetItem(m_flag);
            }
        }
    }
}
