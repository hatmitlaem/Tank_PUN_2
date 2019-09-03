using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundManager : MonoBehaviour
{
    public static GameSoundManager m_instance;
    public AudioSource m_shotAudio;
    public AudioSource m_playerExploisionAudio;
    public AudioSource m_itemAudio;
    void Awake()
    {
        if (!m_instance)
            m_instance = this;
    }

}
