
using UnityEngine;
using UnityEngine.UI;
public class pauseTimer : MonoBehaviour
{
    [SerializeField]
    private Text m_text;
    private bool m_isActive;
    private float m_time;
    [HideInInspector]
    public float m_currentTime;
    [HideInInspector]
    public bool m_timeOut = false;
    public void SetActive(bool flag, float lag = 0, float time = 30)
    {
        m_isActive = flag;
        m_time = time;
        m_currentTime = lag;
        m_text.text = "";
        m_timeOut = false;
    }
    void Update()
    {
        if (!m_isActive)
            return;
        if (m_currentTime >= m_time)
        {
            m_isActive = false;
            m_timeOut = true;
        }
        m_currentTime += Time.deltaTime;
        m_text.text = ((int)Mathf.Max(m_time - m_currentTime, 0)).ToString();
    }
}
