using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using XaviEssencials.Runtime;
using XaviGames.Host;
using XaviGames.Manager;

namespace XaviGames.Ui
{
    public class MatchUIController : NetworkBehaviour
    {
        [field: Header("Scripts References")]
        [field: SerializeField]
        public CanvasGroupController HudCanvasController { get; private set; }

        [field: SerializeField]
        public MatchEndHandler MatchEndHandler { get; private set; }

        [field: SerializeField]
        public CanvasGroupController WaitingPlayersBannerController { get; private set; }

        [SerializeField]
        private SceneReference _menuScene;

        public static MatchUIController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                NetworkManager networkManager = NetworkManager.Singleton;
                if (networkManager != null)
                {
                    networkManager.OnClientDisconnectCallback += OnClientDisconnected;
                    networkManager.OnServerStopped += OnHostStopped;
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public override void OnNetworkSpawn()
        {
            GameManager gameManager = GameManager.Instance; 

            OnGameStateChanged(GameState.Off, gameManager.GameState);
            gameManager.NetState.OnValueChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState previousValue, GameState newValue)
        {
            switch (newValue)
            {
                case GameState.WaitingForPlayers:
                    WaitingPlayersBannerController.EnableCanvas();
                    break;
                case GameState.GameInProgress:
                    WaitingPlayersBannerController.DisableCanvas();
                    HudCanvasController.EnableCanvas();
                    break;
                default:
                    HudCanvasController.DisableCanvas();
                    WaitingPlayersBannerController.DisableCanvas();
                    break;
            }
        }

        private void OnDestroy()
        {
            NetworkManager networkManager = NetworkManager.Singleton;
            if (networkManager != null)
            {
                networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
                networkManager.OnServerStopped -= OnHostStopped;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId)
                return;

            SceneManager.LoadScene(_menuScene.SceneName);
        }

        private void OnHostStopped(bool isHost)
        {
            if (!isHost)
                return;

            NetworkManager networkManager = NetworkManager.Singleton;
            if (networkManager == null)
                return;

            foreach (ulong clientId in networkManager.ConnectedClientsIds.ToList())
            {
                if (clientId == networkManager.LocalClientId)
                    continue;

                networkManager.DisconnectClient(clientId);
            }
        }

        public async void LeaveGameAsync()
        {
            NetworkManager networkManager = NetworkManager.Singleton;
            if (networkManager == null)
                return;

            if (IsHost)
            {
                foreach (ulong clientId in networkManager.ConnectedClientsIds.ToList())
                {
                    if (clientId != networkManager.LocalClientId)
                    {
                        networkManager.DisconnectClient(clientId);
                    }
                }
            }

            networkManager.Shutdown();
            await ClearUp();
        }

        private async Task ClearUp()
        {
            NetworkManager networkManager = NetworkManager.Singleton;
            while (networkManager != null && networkManager.ShutdownInProgress)
            {
                await Task.Yield();
            }
        }
    }
}
