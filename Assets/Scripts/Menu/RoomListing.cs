using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private Text m_roomNameText;
    [SerializeField]
    private Text m_numPlayerText;
    [SerializeField]
    private Text m_numKillText;
    [HideInInspector]
    public string m_roomName;
    [HideInInspector]
    public bool m_updated;

    void Start()
    {
        GameObject lobbyCanvasObj = MainCanvasManager.m_instance.m_lobbyCanvas.gameObject;
        if (!lobbyCanvasObj)
            return;
        LobbyCanvas lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>();

        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => lobbyCanvas.OnClickJoinRoom(m_roomNameText.text));

    }

    void OnDestroy()
    {
        Button b = GetComponent<Button>();
        b.onClick.RemoveAllListeners();
    }

    public void SetRoomNameText(string text)
    {
        m_roomNameText.text = text;
        m_roomName = text;
    }

    public void SetNumPlayerText(string text)
    {
        m_numPlayerText.text = text;
    }

    public void SetNumKillText(string text)
    {
        m_numKillText.text = text;
    }
}
