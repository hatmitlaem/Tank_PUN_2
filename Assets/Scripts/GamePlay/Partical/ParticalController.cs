using System.Collections;
using UnityEngine;

public class ParticalController : MonoBehaviour
{
    ParticleSystem m_partical;
    void Start()
    {
        m_partical = GetComponent<ParticleSystem>();
    }
    
    void Play()
    {
        if (GameUiManager.m_instance.m_isPause)
        {
            if (m_partical.isPlaying)
                m_partical.Pause();
        }
        else
        {
            if (m_partical.isPaused)
                m_partical.Play();
        }
    }
    void Update()
    {
        Play();
    }
}
