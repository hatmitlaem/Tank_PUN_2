using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPun
{
    [SerializeField]
    private LayerMask m_TankMask;
    [SerializeField]
    private ParticleSystem m_ExplosionParticles;
    [SerializeField]
    private AudioSource m_audio;
    [SerializeField]
    private float m_MaxDamage = 100f;
    [SerializeField]
    private float m_ExplosionForce = 1000f;
    [SerializeField]
    private float m_ExplosionRadius = 5f;

    [HideInInspector]
    public int m_teamIndex;
    private bool m_isFirst = false;
    private bool m_isPause = false;
    private Rigidbody m_Rigidbody;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        PauseRigidbody();
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
    void OnTriggerEnter(Collider other)
    {
        if (m_isFirst)
            return;
        m_isFirst = true;
        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody)
                continue;
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
            if (!targetHealth)
                continue;
            if (targetHealth.m_isImmortal)
                continue;
            if (PhotonNetwork.IsMasterClient)
            {
                float damage = CalculateDamage(targetRigidbody.position);
                targetHealth.TakeDamage(damage, m_teamIndex, transform.position, m_ExplosionForce, m_ExplosionRadius);
            }
        }
        m_ExplosionParticles.transform.parent.parent = null;
        m_ExplosionParticles.Play();
        m_audio.Play();
        GameObject.Destroy(gameObject);
    }
    float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        float damage = relativeDistance * m_MaxDamage;
        damage = Mathf.Max(0f, damage);
        return damage;
    }
}

