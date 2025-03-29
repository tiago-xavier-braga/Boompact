using Unity.Netcode;
using UnityEngine;

namespace XaviGames.Multiplayer
{
    public class ClientManager : MonoBehaviour
    {
        private void Start()
        {
            var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
            transport.ConnectionData.Address = "20.33.63.92";
            transport.ConnectionData.Port = (ushort)9000;

            bool isClientStarted = NetworkManager.Singleton.StartClient();
            Debug.Log($"Client Status: {isClientStarted}");
        }
    }
}
