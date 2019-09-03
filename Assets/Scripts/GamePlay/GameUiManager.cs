using UnityEngine;
using UnityEngine.UI;
public class GameUiManager : MonoBehaviour
{
    public static GameUiManager m_instance;
    public Text m_spawnerText;
    [SerializeField]
    private GameObject m_gameOverObj;
    [SerializeField]
    private Text m_gameOverText;
    [HideInInspector]
    public bool m_isPause;
    [HideInInspector]
    public bool m_isGameOver;
    private float m_time = 3;
    [HideInInspector]
    public Text[] m_TeamScoreTexts;
    void Awake()
    {
        if (!m_instance)
            m_instance = this;
        m_gameOverObj.SetActive(false);
        m_isPause = false;
        m_isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerManager.m_instance || m_isPause)
            return;
        if (PlayerManager.m_instance.gameObject.GetComponent<TankHealth>().m_Dead)
        {
            m_time -= Time.deltaTime;
            m_spawnerText.text = ((int)(m_time + 1)).ToString();
        }
        if (m_time<=0)
        {
            m_time = 3;
            PlayerManager.m_instance.gameObject.GetComponent<TankHealth>().Spawner();
            m_spawnerText.text = "";
        }
    }
    public void SetGameOver(int teamWin)
    {
        m_isGameOver = true;
        m_isPause = true;
        m_gameOverObj.SetActive(true);
        switch (teamWin)
        {
            case 0:
                m_gameOverText.text = "Red team win";
                break;
            case 1:
                m_gameOverText.text = "Blue team win";
                break;
            case 2:
                m_gameOverText.text = "Green team win";
                break;
            default:
                m_gameOverText.text = "Yellow team win";
                break;
        }
    }

   

}
