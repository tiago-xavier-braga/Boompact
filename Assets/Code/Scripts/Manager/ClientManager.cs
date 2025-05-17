using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Services;

namespace XaviGames.Manager
{
    public class ClientManager : MonoBehaviour
    {
        [SerializeField]
        private ServicesSettings _servicesSettings;

        [SerializeField]
        private MatchmakerSettings _matchmakerSettings;

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
            var serviceType = _servicesSettings.ClientServiceType;
            if (serviceType == ServiceType.Local)
            {
                ConnectToMockServer();
                return;
            }

            var players = new List<Player>
            {
                new(AuthenticationService.Instance.PlayerId, new Dictionary<string, object>())
            };

            var attributes = new Dictionary<string, object>();
            var options = new CreateTicketOptions(_matchmakerSettings.QueueName, attributes);

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

                GameLogger.Log("Polling", LogCategory.Client);
                
                var ticketStatusResponse = await MatchmakerService.Instance.GetTicketAsync(ticketResponse.Id);
                if (ticketStatusResponse?.Value is MultiplayAssignment assignment)
                {
                    GameLogger.Log($"Response {assignment.Status}", LogCategory.Client);
                    
                    switch (assignment.Status)
                    {
                        case MultiplayAssignment.StatusOptions.Found:
                            {
                                if (assignment.Port.HasValue)
                                {
                                    transport.SetConnectionData(assignment.Ip, (ushort)assignment.Port);
                                    bool result = NetworkManager.Singleton.StartClient();


                                    GameLogger.Log($"Start Client {result}", LogCategory.Client);
                                    return result;
                                }

                                GameLogger.LogError("No port found", LogCategory.Client);
                                return false;
                            }
                        case MultiplayAssignment.StatusOptions.Timeout:
                        case MultiplayAssignment.StatusOptions.Failed:
                            {
                                GameLogger.LogError(assignment.ToString(), LogCategory.Client);
                                return false;
                            }
                    }
                }
            }
        }

        private void ConnectToMockServer()
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(_servicesSettings.TestServerIP, _servicesSettings.TestServerPort);
            var success = NetworkManager.Singleton.StartClient();

            GameLogger.Log($"Client started local connection: {success}", LogCategory.Test);
        }

    }
}