using UnityEngine;
public class ItemManager : MonoBehaviour
{
    public static ItemManager m_instance;
    public GameObject[] m_listItem;
    
    void Awake()
    {
        if (!m_instance)
            m_instance = this;
        SetItem(true);
    }

    public void SetItem(bool flag)
    {
        for (int i = 0; i < m_listItem.Length; i++)
            m_listItem[i].SetActive(flag);
    }

    public void SetItem(bool []list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            m_listItem[i].SetActive(list[i]);
        }
    }
}
