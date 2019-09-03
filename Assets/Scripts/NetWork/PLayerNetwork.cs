using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class PLayerNetwork : MonoBehaviourPunCallbacks
{
    public static PLayerNetwork m_instance;
    // [HideInInspector]
    public string m_name;
    private int m_id;
    [HideInInspector]
    public int m_numPlayerInGame = 0;
    [HideInInspector]
    public int m_teamIndex = -1;
    public PhotonView m_PV;
    void Awake()
    {
        if (!m_instance)
        {
            m_instance = this;
            m_id = PlayerPrefs.GetInt("id", -1);
            if (m_id == -1)
            {
                m_id = Random.Range(1000, 9999);
                m_name = m_id.ToString();
                PlayerPrefs.SetInt("id", m_id);
                PlayerPrefs.SetString("name", m_name);
            }
            else
                m_name = PlayerPrefs.GetString("name");
        }
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        SceneManager.sceneLoaded += OnSceneFinishedLoading;

    }
    public void OnNameChange(Text name)
    {
        if (name.text.Equals(""))
            m_name = m_id.ToString();
        else
            m_name = name.text;
        print("change player name to: " + m_name);
        if (PhotonNetwork.IsConnectedAndReady)
            PhotonNetwork.NickName = m_name;
        PlayerPrefs.SetString("name", m_name);
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            if (isLoaded)
                return;
            isLoaded = true;
            if (PhotonNetwork.IsMasterClient)
                OnMasterLoadedGame();
            else
                OnNonMasterLoadedGame();
        }
    }

    void OnMasterLoadedGame()
    {
        m_PV.RPC("RPC_LoadGameScence", RpcTarget.MasterClient);
        m_PV.RPC("RPC_LoadGameOthers", RpcTarget.Others);
    }

    void OnNonMasterLoadedGame()
    {
        m_PV.RPC("RPC_LoadGameScence", RpcTarget.MasterClient);
    }

    bool isLoaded = false;
    [PunRPC]
    void RPC_LoadGameOthers()
    {
        PhotonNetwork.LoadLevel(1);
    }

    [PunRPC]
    void RPC_LoadGameScence()
    {
        m_numPlayerInGame++;
        if (m_numPlayerInGame == PhotonNetwork.PlayerList.Length)
        {
            print("ALL layer are in game");
            m_PV.RPC("RPC_CreatePlayer", RpcTarget.All, m_numPlayerInGame);
        }
    }

    [PunRPC]
    void RPC_CreatePlayer(int playerInGame)
    {
        Debug.Log("RPC_CreatePlayer");
        Debug.Log(playerInGame);
        m_numPlayerInGame = playerInGame;
        float r = Random.Range(-3, 3);
        Vector3 pos = new Vector3(r, 0, 0);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                m_teamIndex = i;
                Debug.Log(m_teamIndex);
                if (GameSetup.m_instance)
                    pos = GameSetup.m_instance.m_spawnPointsTeam[i].position;
                break;
            }
        }
        PhotonNetwork.Instantiate("Tank", pos, Quaternion.identity, 0);
    }

}
