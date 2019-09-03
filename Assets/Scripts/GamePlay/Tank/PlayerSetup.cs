using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerSetup : MonoBehaviourPun
{
    public Material[] m_listArmor;
    public MeshRenderer[] m_listMesh;
    public Material[] m_listMasterial;
    public PhotonView m_pv;
    public PlayerManager m_playerManager;
    private int m_team = -1;
    private string m_name = "CuongKhom";
    void Start()
    {
        if (m_pv.IsMine)
        {
            Debug.Log("Setup");
            for (int i = 0; i < PLayerNetwork.m_instance.m_numPlayerInGame; i++)
            {
                if (PLayerNetwork.m_instance.m_teamIndex == i)
                {
                    m_team = i;
                    m_name = PLayerNetwork.m_instance.m_name;
                    m_pv.RPC("RPC_Setup", RpcTarget.AllBuffered, m_team, m_name);
                    Debug.Log(m_team);
                    return;
                }
            }
        }
    }

    public void ActiveArmor()
    {
        for (int i = 0; i < m_listMesh.Length; i++)
        {
            m_listMesh[i].material = m_listArmor[m_team];
        }
    }

    public void DetroyArmor()
    {

        for (int i = 0; i < m_listMesh.Length; i++)
        {
            m_listMesh[i].material = m_listMasterial[m_team];
        }

    }
    bool isSetup = false;
    [PunRPC]
    void RPC_Setup(int team, string name)
    {
        if (isSetup)
            return;
        isSetup = true;
        m_team = team;
        if (m_team == -1)
            return;
        //set color
        for (int i = 0; i < m_listMesh.Length; i++)
        {
            m_listMesh[i].material = m_listMasterial[m_team];
        }
        m_playerManager.m_teamIndex = m_team;
        m_playerManager.m_nameText.text = name;
        GameSetup.m_instance.m_listPlayerTeam[m_team] = m_playerManager;
        if (m_pv.IsMine)
            PlayerManager.m_instance.m_teamIndex = m_team;
    }

}
