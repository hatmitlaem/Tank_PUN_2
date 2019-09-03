using UnityEngine;
using UnityEngine.UI;
public class RoomSetting : MonoBehaviour
{
    public static RoomSetting m_instance;
    [HideInInspector]
    public int m_numKill = 5;

    private int[] m_listKill = new int[] { 5, 25, 50 };
    void Awake()
    {
        m_instance = this;
    }
    
    public void Dropdown_IndexChanged(int index)
    {
        m_numKill = m_listKill[index];
    }
}
