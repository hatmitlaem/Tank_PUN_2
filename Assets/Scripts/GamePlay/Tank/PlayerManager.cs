/*  This file is part of the "Tanks Multiplayer" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPun
{
    public static PlayerManager m_instance;

    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;
    public float m_turretTurnSpeed = 50f;
    public float m_MinLaunchForce = 15f;
    public float m_MaxLaunchForce = 30f;
    public float m_MaxChargeTime = 0.75f;
    public float m_ForceY = 3;
    public Text m_nameText;
    [SerializeField]
    private PhotonView m_PV;
    public Transform m_turret;
    [SerializeField]
    private GameObject m_ShellPrebas;
    [SerializeField]
    private GameObject m_PowerShellPre;
    [SerializeField]
    private Transform m_FireTransform;
    [SerializeField]
    private Slider m_AimSlider;
    [SerializeField]
    ParticleSystem m_ShotEffect;

    [HideInInspector]
    public int m_teamIndex = -1;
    [HideInInspector]
    public int m_score;
    [HideInInspector]
    public bool m_isUpdateShell = false;
    private Rigidbody m_Rigidbody;
    private float m_MovementInputValue;         // The current value of the movement input.
    private float m_TurnInputValue;             // The current value of the turn input.
    private float m_turretTurnInputValue;

    private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                       // Whether or not the shell has been launched with this button press.
    private GameObject m_currentShell;
    private bool m_isPause;

    void Awake()
    {
        if (m_PV.IsMine)
        {
            m_instance = this;
        }
        DontDestroyOnLoad(this);
        m_Rigidbody = GetComponent<Rigidbody>();
        m_currentShell = m_ShellPrebas;
        m_score = 0;
        m_isPause = false;
    }
    void Start()
    {
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }
    void OnEnable()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_Rigidbody.isKinematic = false;

        // Also reset the input values.
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;

        //
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }

    void OnDisable()
    {
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;
    }

    void Update()
    {
        if (!m_PV.IsMine || GameUiManager.m_instance.m_isPause)
            return;
        // Store the value of both input axes.
        m_MovementInputValue = Input.GetAxis("Vertical");
        m_TurnInputValue = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.Z))
            m_turretTurnInputValue = 1;
        else if (Input.GetKey(KeyCode.X))
            m_turretTurnInputValue = -1;
        else
            m_turretTurnInputValue = 0;

        GetFireInput();
    }
    void FixedUpdate()
    {
        if (PauseRigidbody())
            return;
        Move();
        Turn();
        TurnTurret();
    }

    //////////////////// pause
    Vector3 savedVelocity;
    Vector3 savedAngularVelocity;
    bool PauseRigidbody()
    {
        if (GameUiManager.m_instance.m_isPause != m_isPause)
        {
            m_isPause = GameUiManager.m_instance.m_isPause;
            if (m_isPause)
            {
                savedVelocity = m_Rigidbody.velocity;
                savedAngularVelocity = m_Rigidbody.angularVelocity;
                m_Rigidbody.isKinematic = true;
            }
            else
            {
                m_Rigidbody.isKinematic = false;
                m_Rigidbody.AddForce(savedVelocity, ForceMode.VelocityChange);
                m_Rigidbody.AddTorque(savedAngularVelocity, ForceMode.VelocityChange);
            }
        }
        return GameUiManager.m_instance.m_isPause;
    }
    ///////////////////////////
    void GetFireInput()
    {
        m_AimSlider.value = m_MinLaunchForce;

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

        }
        else if (Input.GetKey(KeyCode.Space) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetKeyUp(KeyCode.Space) && !m_Fired)
        {
            Fire();
        }
    }
    void Fire()
    {
        m_Fired = true;
        m_PV.RPC("RPC_CreateBullet", RpcTarget.All, m_CurrentLaunchForce);
        m_ShotEffect.Play();
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
    void TurnTurret()
    {
        float turn = m_turretTurnInputValue * m_turretTurnSpeed * Time.deltaTime;
        m_turret.transform.Rotate(new Vector3(0f, turn, 0f));
    }
    void Move()
    {
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }
    void Turn()
    {
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    public void AddExploisionForce(Vector3 pos, float force, float radius)
    {
        if (m_PV.IsMine)
            m_Rigidbody.AddExplosionForce(force, pos, radius);
    }
    public void UpdateScore(int score)
    {
        m_score += score;
        GameUiManager.m_instance.m_TeamScoreTexts[m_teamIndex].text = m_score.ToString();
        if (m_score == (int)PhotonNetwork.CurrentRoom.CustomProperties["NumKill"])
            GameUiManager.m_instance.SetGameOver(m_teamIndex);
    }
    public void SetScore(int score)
    {
        m_score = score;
    }
    public void SetShell(bool isUpdate)
    {
        m_isUpdateShell = isUpdate;
        if (m_isUpdateShell)
            m_currentShell = m_PowerShellPre;
        else
            m_currentShell = m_ShellPrebas;
    }
    public void UpdateShell()
    {
        m_PV.RPC("RPC_UpdateShell", RpcTarget.All);
    }

    public void ResetShell()
    {
        m_currentShell = m_ShellPrebas;
        m_isUpdateShell = false;
    }

    [PunRPC]
    void RPC_CreateBullet(float force)
    {
        GameObject butlet = GameObject.Instantiate(m_currentShell, m_FireTransform.position, m_FireTransform.rotation);
        butlet.GetComponent<Bullet>().m_teamIndex = m_teamIndex;
        Rigidbody rd = butlet.GetComponent<Rigidbody>();
        rd.velocity = force * m_FireTransform.forward;
        rd.velocity = new Vector3(rd.velocity.x, force / m_MaxLaunchForce * m_ForceY, rd.velocity.z);
        GameSoundManager.m_instance.m_shotAudio.Play();
    }

    [PunRPC]
    void RPC_UpdateShell()
    {
        if (PhotonNetwork.LocalPlayer.IsLocal)
            GameSoundManager.m_instance.m_itemAudio.Play();
        m_currentShell = m_PowerShellPre;
        m_isUpdateShell = true;
    }
}
