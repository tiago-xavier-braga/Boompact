using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using XaviGames.Services;

namespace XaviGames.Manager
{
    public class ClientManager : MonoBehaviour
    {
        [SerializeField]
        private ServicesSettings _servicesSettings;

        private bool _initialized;
        public static ClientManager Instance { get; private set; } = null;

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

        private async void Start()
        {
            await StartServices();
        }

        private async Task StartServices()
        {
            if (!_initialized)
            {
                await UnityServices.InitializeAsync();
                AuthenticationService.Instance.SwitchProfile(UnityEngine.Random.Range(0, 1000000).ToString());
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                _initialized = true;
            }
        }

        public async Task StartSearch()
        {
            var players = new List<Player>
            {
                new(AuthenticationService.Instance.PlayerId, new Dictionary<string, object>())
            };

            var attributes = new Dictionary<string, object>();
            var options = new CreateTicketOptions(_servicesSettings.QueueName, attributes);

            while (!await FindMatch(players, options))
            {
                await Awaitable.WaitForSecondsAsync(1f);
            }
        }

        private async Task<bool> FindMatch(List<Player> players, CreateTicketOptions options)
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);

            while (true)
            {
                await Awaitable.WaitForSecondsAsync(1f);
                Debug.Log("Polling");
                var ticketStatusResponse = await MatchmakerService.Instance.GetTicketAsync(ticketResponse.Id);
                if (ticketStatusResponse?.Value is MultiplayAssignment assignment)
                {
                    Debug.Log("Response " + assignment.Status);
                    FindFirstObjectByType<TMP_Text>()?.SetText("Response " + assignment.Status);
                    switch (assignment.Status)
                    {
                        case MultiplayAssignment.StatusOptions.Found:
                            {
                                if (assignment.Port.HasValue)
                                {
                                    transport.SetConnectionData(assignment.Ip, (ushort)assignment.Port);
                                    bool result = NetworkManager.Singleton.StartClient();

                                    Debug.Log("StartClient " + result);
                                    FindFirstObjectByType<TMP_Text>().SetText("StartClient " + result);
                                    //NetworkManager.Singleton.OnConnectionEvent += LogConnectionEvent;

                                    return result;
                                }

                                Debug.LogError("No port found");
                                return false;
                            }
                        case MultiplayAssignment.StatusOptions.Timeout:
                        case MultiplayAssignment.StatusOptions.Failed:
                            {
                                Debug.LogError(assignment.ToString());
                                return false;
                            }
                    }
                }
            }
        }

        ////TODO: Remove the Log
        //private void LogConnectionEvent(NetworkManager manager, ConnectionEventData data)
        //{
        //    switch (data.EventType)
        //    {
        //        case ConnectionEvent.ClientConnected:
        //            FindFirstObjectByType<TMP_Text>().SetText("Client connected " + data.ClientId +
        //                                                      " Count:" +
        //                                                      NetworkManager.Singleton.ConnectedClientsIds.Count + " Port:" +
        //                                                      (manager.NetworkConfig.NetworkTransport as UnityTransport)?.ConnectionData.Port);
        //            break;
        //        case ConnectionEvent.ClientDisconnected:
        //            FindFirstObjectByType<TMP_Text>()
        //                .SetText("Client disconnected " + data.ClientId + " Count:" +
        //                         NetworkManager.Singleton.ConnectedClientsIds.Count + " Port:" +
        //                         (manager.NetworkConfig.NetworkTransport as UnityTransport)?.ConnectionData.Port);
        //            break;
        //    }
        //}
    }
}