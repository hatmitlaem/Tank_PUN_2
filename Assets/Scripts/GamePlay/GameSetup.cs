using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class GameSetup : MonoBehaviourPun
{
    public static GameSetup m_instance; //singleton
    [HideInInspector]
    public int m_nextTeam = 0;
    public Transform[] m_spawnPointsTeam;
    public GameObject[] m_listScoreTeam;
    public PlayerManager[] m_listPlayerTeam = new PlayerManager[4];
    [SerializeField]
    private Text m_numKillText;
    void Awake()
    {
        if(!m_instance)
            m_instance = this;
    }
  
    void Start()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            m_listScoreTeam[i].SetActive(true);
        m_numKillText.text = PhotonNetwork.CurrentRoom.CustomProperties["NumKill"].ToString();

    }
    public void updateTeam()
    {
        m_nextTeam++;
        if (m_nextTeam > 3)
            m_nextTeam = 3;
    }
    // Update is called once per frame
    
}
