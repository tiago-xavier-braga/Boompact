using Unity.Netcode;
using UnityEngine;

namespace XaviGames.Multiplayer
{
    public class ServerManager : MonoBehaviour
    {
        public static ServerManager Instance { get; private set; }

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
            Debug.Log("Starting Server in Batch Mode...");
            if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StartServer();
                Debug.Log("Server started!");
            }
        }
    }
}
