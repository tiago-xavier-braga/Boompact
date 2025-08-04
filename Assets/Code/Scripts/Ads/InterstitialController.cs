using UnityEngine;
using UnityEngine.Advertisements;
using XaviEssencials.Runtime;
using XaviGames.Ui;

namespace XaviGames.Ads
{
    public class InterstitialController : AdsBase, IUnityAdsShowListener
    {
        [SerializeField]
        private CanvasGroupController _canvasGroupController;

        public void OnUnityAdsShowClick(string placementId)
        {
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            _canvasGroupController.DisableCanvas();
            IsLoaded = false;
            Load();
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            _canvasGroupController.DisableCanvas();
            IsLoaded = false;
            Load();
        }

        public void OnUnityAdsShowStart(string placementId)
        {
        }

        public void ShowAd()
        {
            if (!IsLoaded)
            {
                GameLogger.LogError("Ad is not loaded. Please load the ad before showing it.", LogCategory.Unity);
                return;
            }
            if (!Advertisement.isInitialized)
            {
                GameLogger.LogError("Unity Ads is not initialized. Please initialize before showing ads.",
                    LogCategory.Unity);
                return;
            }
            _canvasGroupController.EnableCanvas();
            Advertisement.Show(PlacementId, this);
        }
    }
}
