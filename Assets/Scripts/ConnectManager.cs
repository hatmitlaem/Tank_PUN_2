
using UnityEngine;
using Photon.Pun;
public class ConnectManager : MonoBehaviour
{
    public void OnClick_Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void OnClick_Reconnect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            print("Reconnecting...");
            PhotonNetwork.Reconnect();
        }
    }

    public void OnClick_Rejoin()
    {
        if (!PhotonNetwork.IsConnected)
        {
            print("ReconnectAndRejoin...");
            PhotonNetwork.ReconnectAndRejoin();
        }
    }
}
