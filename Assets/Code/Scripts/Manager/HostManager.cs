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
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using XaviEssencials.Runtime;
using XaviGames.Services;
using XaviGames.Ui;
using System.Collections;

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
        private Guid _allocationId = new();
        private Lobby _lobby;
        private Coroutine _heartbeatCoroutine;

        public static HostManager Instance { get; private set; } = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            NetworkManager.Singleton.OnServerStopped += OnHostStopped;

            DontDestroyOnLoad(gameObject);
        }



        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStopped -= OnHostStopped;
            }
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

            LoadingScenes();
        }

        public void StartMatch()
        {
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
                _allocationId = allocation.AllocationId;
                NetworkManager.Singleton.GetComponent<UnityTransport>()
                    .SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
            
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                _joinCode = joinCode;


                var createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Data = new Dictionary<string, DataObject>
                    {
                        {"joinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode)}
                    }
                };

                _lobby = await LobbyService.Instance.CreateLobbyAsync(
                        Guid.NewGuid().ToString("N").Substring(0, 8),
                         maxConnections,
                         createLobbyOptions
                    );

                _heartbeatCoroutine = StartCoroutine(HeartbeatLobbyCoroutine(_lobby.Id));
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
                $"Join Code {_joinCode}, " +
                $"Lobby: {_lobby.Name}", LogCategory.Relay);

            return true;
        }

        private void LoadingScenes()
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadingHandler;
            NetworkManager.Singleton.SceneManager.LoadScene(_environmentScene.SceneName, LoadSceneMode.Single);
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
            
            MatchUIController matchUIController = MatchUIController.Instance;

            matchUIController.HudCanvasController.EnableCanvas();
            LoadingCanvasController.Instance.DisableLoading();
        }

        private IEnumerator HeartbeatLobbyCoroutine(string lobbyId)
        {
            var delay = new WaitForSeconds(15);

            while (true)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return delay;
            }
        }

        private async void OnHostStopped(bool isHost)
        {
            if (!isHost)
            {
                return;
            }

            await DeleteLobbyAsync();
        }

        private async Task DeleteLobbyAsync()
        {
            if (_lobby != null)
            {
                try
                {
                    await LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    GameLogger.LogError($"Failed to delete lobby [{_lobby.Id}]: {e.Message}", LogCategory.Lobby);
                }
            }
        }
    }
}