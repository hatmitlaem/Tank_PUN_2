using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public static MainCanvasManager m_instance;

    public LobbyCanvas m_lobbyCanvas;
    public CurrentRoomCanvas m_currentRoomCanvas;
    void Awake()
    {
        m_instance = this;
    }
}
