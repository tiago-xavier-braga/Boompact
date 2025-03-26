using Unity.Netcode;
using UnityEngine;

namespace XaviGames.Multiplayer
{
    public class ClientManager : MonoBehaviour
    {
        public static ClientManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Debug.Log("Starting Client...");
            if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StartClient();
                Debug.Log("Client started!");
            }

        }
    }
}
