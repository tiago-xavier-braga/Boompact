//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Ui
{
    public class LoadingController : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _loadingCanvasGroup;

        [SerializeField]
        private LeanTweenType _loadingTweenType = LeanTweenType.easeInOutQuad;

        [SerializeField]
        private float _loadingDuration = 0.5f;

        public static LoadingController Instance { get; private set; }

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

        public void EnableLoading()
        {
            if (_loadingCanvasGroup is null)
            {
                GameLogger.LogError("Loading Canvas Group is null", LogCategory.Client);
                return;
            }

            LeanTween.cancel(gameObject);

            LeanTween.alphaCanvas(_loadingCanvasGroup, 1f, _loadingDuration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() => GameLogger.Log("Loading enabled", LogCategory.Client));

            _loadingCanvasGroup.interactable = true;
            _loadingCanvasGroup.blocksRaycasts = true;
        }

        public void DisableLoading()
        {
            if (_loadingCanvasGroup is null)
            {
                GameLogger.LogError("Loading Canvas Group is null", LogCategory.Client);
                return;
            }

            LeanTween.cancel(gameObject);

            LeanTween.alphaCanvas(_loadingCanvasGroup, 0f, _loadingDuration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() => GameLogger.Log("Loading disabled", LogCategory.Client));

            _loadingCanvasGroup.interactable = false;
            _loadingCanvasGroup.blocksRaycasts = false;
        }
    }
}
