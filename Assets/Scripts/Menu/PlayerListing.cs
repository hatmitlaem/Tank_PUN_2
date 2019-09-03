using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class PlayerListing : MonoBehaviour
{
    [HideInInspector]
    public Player m_photonPlayer;
    public Text m_playerNameText;
    public GameObject m_kickButton;
    public GameObject m_keyIcon;

    public void ApplyPhotonPlayer(Player photonPlayer)
    {
        m_playerNameText.text = photonPlayer.NickName;
        m_photonPlayer = photonPlayer;
    }

    void OnDestroy()
    {
        m_kickButton.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    //public void SetPlayerNameText(string text)
    //{
    //    m_playerNameText.text = text;
    //}
}
