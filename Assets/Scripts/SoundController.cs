using UnityEngine;

public class SoundController : MonoBehaviour
{
    AudioSource m_audio;
    float startVolunm;
    void Start()
    {
        m_audio = GetComponent<AudioSource>();
        startVolunm = m_audio.volume;
        if (SoundManager.m_instance)
        {
            m_audio.volume = startVolunm* SoundManager.m_instance.m_volunm;
        }
    }
    public void OnSliderValueChanged(float value)
    {
        if(m_audio)
            m_audio.volume = startVolunm * value;
    }

}
