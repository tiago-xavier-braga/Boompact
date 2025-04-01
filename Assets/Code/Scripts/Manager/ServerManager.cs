using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;
using XaviEssencials;

namespace XaviGames.Manager
{
    public class ServerManager : MonoBehaviour
    {
        [SerializeField]
        private SceneReference _sceneToLoad;

        [SerializeReference]
        private string _queueName;

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
            Debug.Log("Network Transport " + transport.ConnectionData.Address + ":" + transport.ConnectionData.Port);

            if (!NetworkManager.Singleton.StartServer())
            {
                Debug.Log("Failed to start server");
                throw new Exception("Failed to start server");
            }

            NetworkManager.Singleton.OnClientConnectedCallback += (clientId) => { Debug.Log("Client connected"); };
            NetworkManager.Singleton.OnServerStopped += (reason) => { Debug.Log("Server stopped"); };

            NetworkManager.Singleton.SceneManager.LoadScene(_sceneToLoad.SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            Debug.Log($"Started Server {transport.ConnectionData.Address}:{transport.ConnectionData.Port}");

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
            Debug.Log($"Subscription state changed: {state}");

        }

        private void OnError(MultiplayError error)
        {
            Debug.LogError($"Error received: {error}");
        }

        private async void OnDeallocate(MultiplayDeallocation deallocation)
        {
            Debug.Log($"Deallocation received: {deallocation}");
            await MultiplayService.Instance.UnreadyServerAsync();
        }

        private async void OnAllocate(MultiplayAllocation allocation)
        {
            Debug.Log($"Allocation received: {allocation}");
            await MultiplayService.Instance.ReadyServerForPlayersAsync();
        }

        private async Task CreateBackfillTicket()
        {
            MatchmakingResults results =
                await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();

            Debug.Log(
                $"Environment: {results.EnvironmentId} MatchId: {results.MatchId} MatchProperties: {results.MatchProperties}");

            var backfillTicketProperties = new BackfillTicketProperties(results.MatchProperties);

            string connectionString = MultiplayService.Instance.ServerConfig.IpAddress + ":" +
                                      MultiplayService.Instance.ServerConfig.Port;

            var options = new CreateBackfillTicketOptions(_queueName,
                connectionString,
                new Dictionary<string, object>(),
                backfillTicketProperties);

            Debug.Log("Requesting backfill ticket");
            _ticketId = await MatchmakerService.Instance.CreateBackfillTicketAsync(options);
        }
        private IEnumerator ApproveBackfillTicketEverySecond()
        {
            for (int i = 4; i >= 0; i--)
            {
                Debug.Log($"Waiting {i} seconds to start backfill");
                yield return new WaitForSeconds(1f);
            }

            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (String.IsNullOrWhiteSpace(_ticketId))
                {
                    Debug.Log("No backfill ticket to approve");
                    continue;
                }

                Debug.Log("Doing backfill approval for _ticketId: " + _ticketId);
                yield return MatchmakerService.Instance.ApproveBackfillTicketAsync(_ticketId);
                Debug.Log("Approved backfill ticket: " + _ticketId);
            }
        }
    }
}
