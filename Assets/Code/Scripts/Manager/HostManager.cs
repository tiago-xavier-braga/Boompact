// Boompact (c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using XaviEssencials.Runtime;
using XaviGames.Services;
using XaviGames.Ui;

namespace XaviGames.Host
{
    public sealed class HostManager : MonoBehaviour
    {
        [field: SerializeField]
        [field: ReadOnly]
        public HostState HostState { get; private set; } = HostState.Off;

        [SerializeField]
        private HostSettings _hostSettings;

        [Header("Services")]
        [SerializeField]
        private MatchController _matchController;

        [Header("Scenes")]
        [SerializeField]
        private SceneReference _environmentScene;

        private string _joinCode = string.Empty;
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

        public async void StartHostWithRelay(Action<bool> callback)
        {
            bool isSuccess = await StartHostWithRelay(_hostSettings.MaxPlayersInMatch, _hostSettings.GetConnectionType());

            if (isSuccess)
            {
                SetServerState(HostState.WaitingForPlayers);
            }
            else
            {
                SetServerState(HostState.Off);
            }
            
            callback?.Invoke(isSuccess);
        }

        public async void StartMatch()
        {
            LoadingCanvasController.Instance.EnableLoading();

            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadingHandler;
            NetworkManager.Singleton.SceneManager.LoadScene(_environmentScene.SceneName, LoadSceneMode.Single);

            _matchController.StartMatch();
        }

        private async Task<bool> StartHostWithRelay(int maxConnections, string connectionType)
        {
            try
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
                _joinCode = joinCode;
            }
            catch (System.Exception e)
            {
                GameLogger.LogError($"Failed to start host with Relay: {e.Message}", LogCategory.Relay);
                return false;
            }

            if (!NetworkManager.Singleton.StartHost())
            {
                GameLogger.LogError("Failed to start host.", LogCategory.Server);
                return false;
            }

            GameLogger.Log($"Starting host with Relay." +
                $" Max players: {_hostSettings.MaxPlayersInMatch}, " +
                $"Connection type: {_hostSettings.GetConnectionType()}. " +
                $"Join Code {_joinCode}", LogCategory.Relay);

            return true;
        }

        private void OnSceneLoadingHandler(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (NetworkManager.Singleton.LocalClientId != clientId)
            {
                return;
            }

            if (sceneName != _environmentScene.SceneName)
            { 
                return;
            }

            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoadingHandler;

            LoadingCanvasController.Instance.DisableLoading();
        }

    }
}