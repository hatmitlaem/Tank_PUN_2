using UnityEngine;
using UnityEngine.UI;
public class SoundManager : MonoBehaviour
{
    public static SoundManager m_instance;
    [HideInInspector]
    public float m_volunm = 1;
    [SerializeField]
    private Slider m_volunmSlider;
    void Awake()
    {
        if (!m_instance)
            m_instance = this;
        if (PlayerPrefs.GetFloat("volunm", -1) == -1)
            PlayerPrefs.SetFloat("volunm", m_volunm);
        else
            m_volunm = PlayerPrefs.GetFloat("volunm");
        m_volunmSlider.value = m_volunm;

    }

    public void OnSliderValueChanged(float value)
    {
        m_volunm = value;
        PlayerPrefs.SetFloat("volunm", m_volunm);
    }

}
