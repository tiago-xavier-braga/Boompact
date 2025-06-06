// Boompact (c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

#if UNITY_SERVER
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using UnityEngine;
using XaviEssencials.Runtime;
using Unity.Services.Multiplay;
using XaviGames.Services;
using UnityEngine.Events;
using XaviGames.Manager;
using System.Collections.Generic;
using UnityEditor;

namespace XaviGames.Server
{
    public sealed class ServerManager : MonoBehaviour
    {
        [field: SerializeField]
        [field: ReadOnly]
        public ServerState ServerState { get; private set; } = ServerState.ServerOff;

        [SerializeField]
        private ServicesSettings _servicesSettings;

        [SerializeField]
        private MatchmakerSettings _matchmakerSettings;

        [SerializeField]
        private MultiplayEventHandler _multiplayEventHandler;

        [SerializeField]
        private BackfillController _backfillController;

        [SerializeField]
        private MatchController _MatchController;

        [SerializeField]
        private NetworkSceneLoader _networkSceneLoader;

        [SerializeField]
        private ConnectedPlayersController _connectedPlayersController;

        [SerializeField]
        public CarSpawnController CarSpawnController { get; private set; }

        [SerializeField]
        private SceneReference _serverScene;

        [SerializeField]
        private SceneBundle _clientSceneBundle;

        public UnityAction<ServerState> OnChangeServerState;

        private async void Start()
        {
            DontDestroyOnLoad(gameObject);
            await StartServer();
        }

        public void SetServerState(ServerState state)
        {
            ServerState = state;
            GameLogger.Log($"Server state changed to: {state}", LogCategory.Server);
        }

        public async void StartMatch()
        {
            await _backfillController.DeleteBackfillTicket();
            SetServerState(ServerState.StartingGame);
            _MatchController.StartMatch();
        }

        public async void ResetMatch()
        {
            OnDisconnectedAllPlayers();
            SetServerState(ServerState.WaitingForPlayers);
            await _backfillController.CreateBackfillTicket();
            _connectedPlayersController.UpdatePlayerCount();
        }

        private async Task StartServer()
        {
            await InitializeUnityServicesAndTransport();
            StartNetworkServer();
            await _networkSceneLoader.LoadSceneAsyncServer(_serverScene);
            await SubscribeMultiplayCallbacksIfNeeded();
            SetServerState(ServerState.WaitingForPlayers);
            await _backfillController.CreateBackfillTicket();
        }

        private async Task InitializeUnityServicesAndTransport()
        {
            await UnityServices.InitializeAsync();

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport is null)
            {
                GameLogger.LogError("UnityTransport component not found on NetworkManager", LogCategory.Server);
                throw new Exception("UnityTransport component not found on NetworkManager");
            }

            if (_servicesSettings.ServerServiceType == ServiceType.Local)
            {
                transport.SetConnectionData("0.0.0.0", _servicesSettings.TestServerPort);
                GameLogger.LogWarning($"Starting the server in LOCAL mode on the port:" +
                    $" {_servicesSettings.TestServerPort}", LogCategory.Server);
            }
            else
            {
                var server = MultiplayService.Instance.ServerConfig;
                transport.SetConnectionData("0.0.0.0", server.Port);
                GameLogger.Log($"Starting Unity Hosting server on port: {server.Port}", LogCategory.Server);
            }

            GameLogger.Log($"Server started at {transport.ConnectionData.Address}:{transport.ConnectionData.Port}",
                LogCategory.Server);
        }

        private void StartNetworkServer()
        {
            if (!NetworkManager.Singleton.StartServer())
            {
                GameLogger.LogError("Failed to start server", LogCategory.Server);
                throw new Exception("Failed to start server");
            }
        }

        private async Task SubscribeMultiplayCallbacksIfNeeded()
        {
            if (_servicesSettings.ServerServiceType != ServiceType.Local)
            {
                var callbacks = new MultiplayEventCallbacks();
                callbacks.Allocate += _multiplayEventHandler.OnServerAllocated;
                callbacks.Deallocate += _multiplayEventHandler.OnServerDeallocated;
                callbacks.Error += _multiplayEventHandler.OnServerErrorReceived;
                callbacks.SubscriptionStateChanged += _multiplayEventHandler.OnServerSubscriptionStateChanged;

                while (MultiplayService.Instance == null)
                {
                    await Awaitable.NextFrameAsync();
                }

                await MultiplayService.Instance.SubscribeToServerEventsAsync(callbacks);
            }
        }

        private void OnDisconnectedAllPlayers()
        {
            List<ulong> clientsToDisonnect = new List<ulong>();

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client != NetworkManager.Singleton.LocalClient)
                {
                    clientsToDisonnect.Add(client.ClientId);
                }
            }

            foreach (var clientId in clientsToDisonnect)
            {
                NetworkManager.Singleton.DisconnectClient(clientId);
            }

            GameLogger.Log("All players disconnected from the server.", LogCategory.Server);
        }
    }
}
#endif
