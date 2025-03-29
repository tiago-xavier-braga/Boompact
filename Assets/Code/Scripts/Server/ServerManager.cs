using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using XaviEssencials;

namespace XaviGames.Multiplayer
{
    public class ServerManager : MonoBehaviour
    {

        [SerializeField]
        private SceneReference _sceneReference;

        [SerializeField]
        public UnityEvent OnClientCallback;

        public static ServerManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(gameObject);
            }
        }

        //private void Start()
        //{
        //    if (NetworkManager.Singleton.IsServer)
        //    {
        //        return;
        //    }

        //    if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        //    {
        //        var serverConfig = MultiplayerService.Instance.ServerConfig;
        //        Debug.Log($"Server ID[{serverConfig.ServerId}]");
        //        Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
        //        Debug.Log($"Port[{serverConfig.Port}]");
        //        Debug.Log($"QueryPort[{serverConfig.QueryPort}]");
        //        Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");

        //        string ipv4Address = "0.0.0.0";
        //        ushort port = serverConfig.Port;
        //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port, "0.0.0.0");

        //        bool isStartedServer = NetworkManager.Singleton.StartServer();
        //        Debug.LogWarning($"Server Status: {isStartedServer}");
        //    }

        //    NetworkManager.Singleton.SceneManager.LoadScene(_sceneReference.SceneName, LoadSceneMode.Single);
        //    NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        //}

        //public static void LogServerConfig()
        //{
        //    var serverConfig = MultiplayService.Instance.ServerConfig;
        //    Debug.Log($"Server ID[{serverConfig.ServerId}]");
        //    Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
        //    Debug.Log($"Port[{serverConfig.Port}]");
        //    Debug.Log($"QueryPort[{serverConfig.QueryPort}");
        //    Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");
        //}

        private void OnClientConnectedCallback(ulong clientId)
        {
            Debug.Log($"Client connected: {clientId}");
            OnClientCallback?.Invoke();
        }
    }
}
