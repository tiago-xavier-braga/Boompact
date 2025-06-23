// Boompact (c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System;
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

namespace XaviGames.Manager
{
    public class ClientManager : MonoBehaviour
    {
        [SerializeField]
        private HostSettings _hostSettings;

        [Header("Scenes")]
        [SerializeField]
        private SceneReference _environmentScene;

        private bool _initialized = false;
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

        private void Start()
        {
            InitializeUnityServicesAsync();
            CanvasManager.Instance.LoadingCanvasController.DisableLoading();
        }

        private async Task InitializeUnityServicesAsync()
        {
            if (!_initialized)
            {
                await UnityServices.InitializeAsync();
                AuthenticationService.Instance.SwitchProfile(UnityEngine.Random.Range(0, 1000000).ToString());
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                _initialized = true;
            }
        }

        public async void StartClientWithRelay(string joinCode, Action<bool> callback)
        {
            bool isSuccess = await StartClientWithRelay(joinCode, _hostSettings.GetConnectionType());
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadingHandler;
            callback?.Invoke(isSuccess);
        }

        private async Task<bool> StartClientWithRelay(string joinCode, string connectionType)
        {
            try
            {
                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
            }
            catch (System.Exception e)
            {
                GameLogger.LogError($"Failed to start client with relay: {e.Message}", LogCategory.Client);
                CanvasManager.Instance.LoadingCanvasController.DisableLoading();
                return false;
            }

            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
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
            CanvasManager.Instance.DisableAllCanvasGroups();
            CanvasManager.Instance.LoadingCanvasController.DisableLoading();
        }
    }
}