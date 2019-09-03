using UnityEngine;
using UnityEngine.UI;
public class ReconnectUI : MonoBehaviour
{
    [SerializeField]
    private GameObject m_reconnectObj;
    [SerializeField]
    private pauseTimer m_pauseTimer;
    [SerializeField]
    private Text m_ReconnectText;
    public void SetActive(bool flag, bool isMaster = false, float lag = 0, float time = 30)
    {
        m_pauseTimer.SetActive(flag &&(!isMaster), lag, time);
        m_reconnectObj.SetActive(flag);
        if(isMaster)
            m_ReconnectText.text = "Disconnect";
        else
            m_ReconnectText.text = "Connecting...";
    }

    void Update()
    {
        if (m_pauseTimer.m_timeOut)
            m_ReconnectText.text = "Disconnect";
    }

}
