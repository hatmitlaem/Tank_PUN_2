using UnityEngine;

public class DestroyPartical : MonoBehaviour
{
    ParticleSystem m_particle;
    bool m_isStated = false;
    void Start()
    {
        m_particle = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        if (!m_isStated)
            m_isStated = m_particle.isPlaying;
        if(m_isStated)
        {
            if (m_particle.isStopped)
                GameObject.Destroy(gameObject);
        }
    }
}
