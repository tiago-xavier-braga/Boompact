using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using XaviEssencials.Runtime;
using XaviGames.Services;

namespace XaviGames.Ads
{
    public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
    {
        [SerializeField]
        private GameSettings _gameSettings;

        [SerializeField]
        private InterstitialController _interstitialController;

        private string _gameId;
        private string _interstitialId;

        public static AdsManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAds();
        }

        public void InitializeAds()
        {
            SetPlatform();

            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, _gameSettings.TestMode, this);
            }
        }

        public void OnInitializationComplete()
        {
            LoadAllAds();
            GameLogger.Log("Unity Ads initialization complete.", LogCategory.Unity);
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            GameLogger.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}", LogCategory.Unity);
        }

        private void SetPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    _gameId = _gameSettings.AndroidGameId;
                    _interstitialId = _gameSettings.InterstitialAndroidId;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    _gameId = _gameSettings.IOSGameId;
                    _interstitialId = _gameSettings.InterstitialIOSId;
                    break;
                default:
                    _gameId = _gameSettings.AndroidGameId;
                    _interstitialId = _gameSettings.InterstitialAndroidId;
                    break;
            }
        }

        private void LoadAllAds()
        {
            _interstitialController.InitializeAds(_interstitialId);
        }
    }
}
