// Boompact (c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Services;
using XaviGames.Ui;

namespace XaviGames.Server
{
    public sealed class HostManager : MonoBehaviour
    {
        [field: SerializeField]
        [field: ReadOnly]
        public HostState HostState { get; private set; } = HostState.Off;

        [SerializeField]
        private HostSettings _hostSettings;

        private static string _joinCode = null;

        public static HostManager Instance { get; private set; } = null;

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

        public void SetServerState(HostState state)
        {
            HostState = state;
            GameLogger.Log($"Server state changed to: {state}", LogCategory.Server);
        }

        public void StartHostWithRelay()
        {
            LoadingCanvasController.Instance.EnableLoading();
            _ = StartHostWithRelay(_hostSettings.MaxPlayersInMatch, _hostSettings.GetConnectionType());
            GameLogger.Log($"Starting host with Relay. Max players: {_hostSettings.MaxPlayersInMatch}," +
                $" Connection type: {_hostSettings.GetConnectionType()}. Join Code {_joinCode}", LogCategory.Server);
        }

        private async Task StartHostWithRelay(int maxConnections, string connectionType)
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            if (NetworkManager.Singleton.StartHost())
            {
                _joinCode = joinCode;
            }
            else
            {
                GameLogger.LogError("Failed to start host.", LogCategory.Server);
                LoadingCanvasController.Instance.DisableLoading();
            }
        }
    }
}