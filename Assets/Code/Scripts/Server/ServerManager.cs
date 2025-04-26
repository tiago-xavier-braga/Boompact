// Boompact (c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

#if UNITY_SERVER
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using XaviEssencials.Runtime;
using Unity.Services.Multiplay;
using XaviGames.Services;

namespace XaviGames.Server
{
    public class ServerManager : MonoBehaviour
    {
        [SerializeField]
        private ServicesSettings _servicesSettings;

        [SerializeField]
        private MultiplayEventHandler _multiplayEventHandler;

        [SerializeField]
        private BackfillController _backfillController;

        [SerializeField]
        private SceneReference _sceneToLoad;

        private async void Start()
        {
            DontDestroyOnLoad(gameObject);

            await StartServer();
            await _backfillController.ApproveBackfillTicketEverySecond();
        }

        private async Task StartServer()
        {
            await InitializeUnityServicesAndTransport();
            StartNetworkServer();

            NetworkManager.Singleton.SceneManager.LoadScene(_sceneToLoad.SceneName,
                UnityEngine.SceneManagement.LoadSceneMode.Single);

            SetupEventCallbacks();
            await SubscribeMultiplayCallbacksIfNeeded();
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

            if (_servicesSettings.BuildServiceType == ServiceType.Local)
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

        private void SetupEventCallbacks()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
            {
                GameLogger.Log($"Client connected {clientId}", LogCategory.Server);
            };

            NetworkManager.Singleton.OnServerStopped += (reason) =>
            {
                GameLogger.Log("Server stopped", LogCategory.Server);
            };
        }

        private async Task SubscribeMultiplayCallbacksIfNeeded()
        {
            if (_servicesSettings.BuildServiceType != ServiceType.Local)
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
    }
}
#endif
