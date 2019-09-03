using UnityEngine;
using Photon.Pun;
public class PlayerLagCompensation : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody m_rd;
    public PhotonView PV;
    public float m_maxDeltaDistance = 15;
    public float m_maxDeltaAngle = 15;
    public Transform m_turret;

    Vector3 m_networkPosition;
    Quaternion m_networkRotation;
    Quaternion m_networkTurretRotaion;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_rd.position);
            stream.SendNext(m_rd.velocity);
            stream.SendNext(m_rd.rotation);
            stream.SendNext(m_turret.rotation);
        }
        else
        {
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            m_networkPosition = (Vector3)stream.ReceiveNext();
            m_rd.velocity = (Vector3)stream.ReceiveNext();
            m_networkRotation = (Quaternion)stream.ReceiveNext();
            m_networkTurretRotaion = (Quaternion)stream.ReceiveNext();
            m_networkPosition += m_rd.velocity * lag;
            if (Vector3.Distance(m_rd.position, m_networkPosition) > m_maxDeltaDistance)
            {
                m_rd.position = m_networkPosition;
            }
        }
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            m_rd.position = Vector3.MoveTowards(m_rd.position, m_networkPosition, Time.deltaTime * PlayerManager.m_instance.m_Speed);
            m_rd.rotation = Quaternion.RotateTowards(m_rd.rotation, m_networkRotation,
                                                        Time.deltaTime * PlayerManager.m_instance.m_TurnSpeed);
        }
    }

    void Update()
    {
        if (!PV.IsMine)
            m_turret.rotation = Quaternion.RotateTowards(m_turret.rotation, m_networkTurretRotaion, Time.deltaTime * PlayerManager.m_instance.m_turretTurnSpeed);
    }
}
