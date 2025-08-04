using UnityEngine;
using UnityEngine.Advertisements;
using XaviEssencials.Runtime;


namespace XaviGames.Ads
{
    public class AdsBase : MonoBehaviour, IUnityAdsLoadListener
    {
        [HideInInspector]
        public bool IsLoaded;

        [HideInInspector]
        public string PlacementId;

        public void OnUnityAdsAdLoaded(string placementId)
        {
            IsLoaded = true;
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            GameLogger.LogError($"Failed to load ad for placement {placementId}: {error} - {message}",
                LogCategory.Unity);
        }

        public void InitializeAds(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                GameLogger.LogError("Ad ID is not set. Please provide a valid ID.", LogCategory.Unity);
                return;
            }

            PlacementId = id;
            Load();
        }

        public void Load()
        {
            if (!Advertisement.isInitialized)
            {
                GameLogger.LogError("Unity Ads is not initialized. Please initialize before loading ads.",
                    LogCategory.Unity);
                return;
            }

            Advertisement.Load(PlacementId, this);
        }
    }
}
