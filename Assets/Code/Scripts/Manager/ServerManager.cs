#if UNITY_SERVER
using System;
using System.Collections;
using System.Collections.Generic;
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
            var server = MultiplayService.Instance.ServerConfig;
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData("0.0.0.0", server.Port);

            GameLogger.Log($"Network Transport {transport.ConnectionData.Address} " +
                $"{transport.ConnectionData.Port}", LogCategory.Server);

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

            //TODO: Modify Load Server Scene
            NetworkManager.Singleton.SceneManager.LoadScene(_sceneToLoad.SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            
            GameLogger.Log($"Started Server {transport.ConnectionData.Address}:" +
                $"{transport.ConnectionData.Port}", LogCategory.Server);

            var callbacks = new MultiplayEventCallbacks();
            callbacks.Allocate += OnAllocate;
            callbacks.Deallocate += OnDeallocate;
            callbacks.Error += OnError;
            callbacks.SubscriptionStateChanged += OnSubscriptionStateChanged;

            while (MultiplayService.Instance == null)
            {
                await Awaitable.NextFrameAsync();
            }

            var events = await MultiplayService.Instance.SubscribeToServerEventsAsync(callbacks);
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
            await MultiplayService.Instance.UnreadyServerAsync();
        }

        private async void OnAllocate(MultiplayAllocation allocation)
        {
            GameLogger.LogWarning($"Allocation received: {allocation}", LogCategory.Server);
            await MultiplayService.Instance.ReadyServerForPlayersAsync();
        }


        private async Task CreateBackfillTicket()
        {
            MatchmakingResults results =
                await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();

            GameLogger.Log($"Environment: {results.EnvironmentId} MatchId: {results.MatchId}" +
                $" MatchProperties: {results.MatchProperties}", LogCategory.Matchmaker);

            var backfillTicketProperties = new BackfillTicketProperties(results.MatchProperties);

            string connectionString = MultiplayService.Instance.ServerConfig.IpAddress + ":" +
                                      MultiplayService.Instance.ServerConfig.Port;

            var options = new CreateBackfillTicketOptions(_servicesSettings.QueueName,
                connectionString,
                new Dictionary<string, object>(),
                backfillTicketProperties);

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