
using UnityEngine;
using Photon.Pun;
public class Powerup : MonoBehaviourPun
{
    public PhotonView m_PV;
    public virtual bool Apply(GameObject p) { return false; }

    Animator m_anim;
    void Start()
    {
        m_anim = GetComponent<Animator>();
    }

    void Update()
    {
        m_anim.enabled = !GameUiManager.m_instance.m_isPause;
    }
    public void Hide()
    {
        m_PV.RPC("RPC_Hide", RpcTarget.All);
    }
    public void Show()
    {
        m_PV.RPC("RPC_Show", RpcTarget.All);
    }

    [PunRPC]
    protected void RPC_Hide()
    {
        gameObject.SetActive(false);
    }

    [PunRPC]
    protected void RPC_Show()
    {
        gameObject.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (other.tag == "Player" || other.tag == "Bullet")
        {
            Apply(other.gameObject);
        }
    }
}

