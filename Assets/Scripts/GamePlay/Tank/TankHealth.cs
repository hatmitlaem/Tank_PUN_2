using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class TankHealth : MonoBehaviourPun
{
    [SerializeField]
    PlayerManager m_playerManager;
    [SerializeField]
    private PhotonView m_PV;
    [SerializeField]
    private float m_StartingHealth = 100f;
    [SerializeField]
    private Slider m_Slider;
    [SerializeField]
    private Image m_FillImage;
    [SerializeField]
    private PlayerSetup m_Setup;
    [SerializeField]
    private GameObject m_ExplosionParticles;
    [HideInInspector]
    public bool m_Dead;
    [HideInInspector]
    public bool m_isImmortal;
    [HideInInspector]
    public bool m_isArmor;
    [HideInInspector]
    public float m_CurrentHealth;


    public void Spawner()
    {
        m_Dead = false;
        m_PV.RPC("RPC_Spawner", RpcTarget.AllBuffered);
    }
    void Awake()
    {
        m_isArmor = false;
        m_isImmortal = false;
    }
    void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
        SetHealthUI();
    }
    void SetHealthUI()
    {
        m_Slider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(Color.red, Color.green, m_CurrentHealth / m_StartingHealth);
    }
    void OnDeath()
    {
        m_isImmortal = true;
        m_Dead = true;
        gameObject.SetActive(false);
        m_playerManager.ResetShell();
        gameObject.transform.rotation = Quaternion.identity;
        m_playerManager.m_turret.rotation = Quaternion.identity;
    }
    public void TakeDamage(float dame, int team, Vector3 pos, float force , float radius)
    {
        if (m_isArmor)
            m_PV.RPC("RPC_DestroyArmor", RpcTarget.All);
        else
            m_PV.RPC("RPC_TakeDame", RpcTarget.All, dame, team, pos ,force ,radius);
    }
    public void AddHealth(float health)
    {
        m_PV.RPC("RPC_AddHealth", RpcTarget.All, health);
    }
    public void ActiveArmor()
    {
        m_PV.RPC("RPC_ActiveArmor", RpcTarget.All);
    }

    public void SetHealth(float health)
    {
        m_CurrentHealth = health;
        SetHealthUI();
    }
    public void SetImmortal(bool update)
    {
        m_isImmortal = update;
        if(m_isImmortal)
            StartCoroutine(UnableImmortalState());
    }
    public void SetArmor(bool update)
    {
        m_isArmor = update;
        if (m_isArmor)
            m_Setup.ActiveArmor();
        else
            m_Setup.DetroyArmor();
    }
    public void SetDead(bool isDead)
    {
        if (isDead)
            OnDeath();
    }
    IEnumerator UnableImmortalState()
    {
        yield return new WaitForSeconds(3);
        m_isImmortal = false;
    }
    // rpc
    [PunRPC]
    void RPC_TakeDame(float dame, int team,Vector3 pos,float force,float radius)
    {
        m_CurrentHealth -= dame;
        SetHealthUI();
        m_playerManager.AddExploisionForce(pos, force, radius);
        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            GameObject exploision = Instantiate(m_ExplosionParticles, transform.position, Quaternion.identity);
            ParticleSystem partical = exploision.GetComponent<ParticleSystem>();
            partical.Play();
            GameSoundManager.m_instance.m_playerExploisionAudio.Play();
            OnDeath();
            if (team == m_playerManager.m_teamIndex)
                GameSetup.m_instance.m_listPlayerTeam[team].UpdateScore(-1);
            else
                GameSetup.m_instance.m_listPlayerTeam[team].UpdateScore(1);

        }
    }
    [PunRPC]
    void RPC_Spawner()
    {
        gameObject.SetActive(true);
        gameObject.transform.position = GameSetup.m_instance.m_spawnPointsTeam[m_playerManager.m_teamIndex].position;
        StartCoroutine(UnableImmortalState());
    }
    [PunRPC]
    void RPC_AddHealth(float health)
    {
        if (PhotonNetwork.LocalPlayer.IsLocal)
            GameSoundManager.m_instance.m_itemAudio.Play();
        m_CurrentHealth += health;
        if (m_CurrentHealth > m_StartingHealth)
            m_CurrentHealth = m_StartingHealth;
        SetHealthUI();
    }
    [PunRPC]
    void RPC_ActiveArmor()
    {
        if (PhotonNetwork.LocalPlayer.IsLocal)
            GameSoundManager.m_instance.m_itemAudio.Play();
        m_isArmor = true;
        m_Setup.ActiveArmor();
    }
    [PunRPC]
    void RPC_DestroyArmor()
    {
        m_isArmor = false;
        m_Setup.DetroyArmor();
    }
}
