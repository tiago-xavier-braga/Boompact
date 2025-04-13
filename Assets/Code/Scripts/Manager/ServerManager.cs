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

namespace XaviGames.Manager
{
    public class ServerManager : MonoBehaviour
    {
        [SerializeField]
        private ServicesSettings _servicesSettings;

        [SerializeField]
        private SceneReference _sceneToLoad;

        private string _ticketId;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            StartCoroutine(StartServer());
            StartCoroutine(ApproveBackfillTicketEverySecond());
        }

        private async Awaitable StartServer()
        {
            await UnityServices.InitializeAsync();

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

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

            if (!NetworkManager.Singleton.StartServer())
            {
                GameLogger.LogError("Failed to start server", LogCategory.Server);
                throw new Exception("Failed to start server");
            }

            NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
            {
                GameLogger.Log("Client connected", LogCategory.Server);
            };

            NetworkManager.Singleton.OnServerStopped += (reason) =>
            {
                GameLogger.Log("Server stopped", LogCategory.Server);
            };

            NetworkManager.Singleton.SceneManager.LoadScene(_sceneToLoad.SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);

            GameLogger.Log($"Server started at {transport.ConnectionData.Address}:{transport.ConnectionData.Port}", LogCategory.Server);

            if (_servicesSettings.BuildServiceType != ServiceType.Local)
            {
                var callbacks = new MultiplayEventCallbacks();
                callbacks.Allocate += OnAllocate;
                callbacks.Deallocate += OnDeallocate;
                callbacks.Error += OnError;
                callbacks.SubscriptionStateChanged += OnSubscriptionStateChanged;

                while (MultiplayService.Instance == null)
                {
                    await Awaitable.NextFrameAsync();
                }

                await MultiplayService.Instance.SubscribeToServerEventsAsync(callbacks);
            }

            await CreateBackfillTicket();
        }

        private void OnSubscriptionStateChanged(MultiplayServerSubscriptionState state)
        {
            GameLogger.LogWarning($"Subscription state changed: {state}", LogCategory.Server);
        }

        private void OnError(MultiplayError error)
        {
            GameLogger.LogError($"Error received: {error}", LogCategory.Server);
        }

        private async void OnDeallocate(MultiplayDeallocation deallocation)
        {
            GameLogger.LogWarning($"Deallocation received: {deallocation}", LogCategory.Server);

            if (_servicesSettings.BuildServiceType != ServiceType.Local)
            {
                await MultiplayService.Instance.UnreadyServerAsync();
            }
        }

        private async void OnAllocate(MultiplayAllocation allocation)
        {
            GameLogger.LogWarning($"Allocation received: {allocation}", LogCategory.Server);

            if (_servicesSettings.BuildServiceType != ServiceType.Local)
            {
                await MultiplayService.Instance.ReadyServerForPlayersAsync();
            }
        }

        private async Task CreateBackfillTicket()
        {
            var serviceType = _servicesSettings.BuildServiceType;
            MatchmakingResults results = new();

            if (serviceType == ServiceType.Local)
            {
                string json = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "MockMultiplayPayload.json"));
                results = JsonUtility.FromJson<MatchmakingResults>(json);
            }
            else
            {
                results = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();
            }

            GameLogger.Log($"Environment: {results.EnvironmentId} MatchId: {results.MatchId}" +
                $" MatchProperties: {results.MatchProperties}", LogCategory.Matchmaker);

            var backfillTicketProperties = new BackfillTicketProperties(results.MatchProperties);

            string connectionString = serviceType == ServiceType.Local
                ? $"{_servicesSettings.TestServerIP}:{_servicesSettings.TestServerPort}"
                : MultiplayService.Instance.ServerConfig.IpAddress + ":" + MultiplayService.Instance.ServerConfig.Port;

            var options = new CreateBackfillTicketOptions(
                _servicesSettings.QueueName,
                connectionString,
                new Dictionary<string, object>(),
                backfillTicketProperties
            );

            GameLogger.Log("Requesting backfill ticket", LogCategory.Matchmaker);
            _ticketId = await MatchmakerService.Instance.CreateBackfillTicketAsync(options);
        }

        private IEnumerator ApproveBackfillTicketEverySecond()
        {
            const int delayBeforeStart = 5;

            for (int i = delayBeforeStart - 1; i >= 0; i--)
            {
                GameLogger.Log($"Waiting {i} seconds to start backfill", LogCategory.Matchmaker);
                yield return new WaitForSeconds(1f);
            }

            while (true)
            {
                yield return new WaitForSeconds(1f);

                if (string.IsNullOrWhiteSpace(_ticketId))
                {
                    GameLogger.Log("No backfill ticket to approve", LogCategory.Matchmaker);
                    continue;
                }

                GameLogger.Log($"Attempting backfill approval for ticket: {_ticketId}", LogCategory.Matchmaker);

                var approvalOperation = MatchmakerService.Instance.ApproveBackfillTicketAsync(_ticketId);
                yield return approvalOperation;

                if (approvalOperation.IsFaulted)
                {
                    GameLogger.LogError($"Failed to approve backfill ticket: {_ticketId}", LogCategory.Matchmaker);
                }
                else
                {
                    GameLogger.Log($"Approved backfill ticket: {_ticketId}", LogCategory.Matchmaker);
                }
            }
        }
    }
}
#endif
