using UnityEngine;
using UnityEngine.Advertisements;
using XaviGames.Services;

namespace XaviGames.Ads
{
    public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
    {
        [SerializeField]
        private GameSettings _gameSettings;

        private string _gameId;

        public static AdsManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            InitializeAds();
        }

        public void InitializeAds()
        {
#if UNITY_IOS
            _gameId = _gameSettings.IOSGameId;
#elif UNITY_ANDROID
            _gameId = _gameSettings.AndroidGameId;
#elif UNITY_EDITOR
        _gameId = _gameSettings.AndroidGameId;
#endif

            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, _gameSettings.TestMode, this);
            }
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }
    }
}
