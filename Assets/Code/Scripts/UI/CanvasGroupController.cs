using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Ui
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupController : MonoBehaviour
    {
        [SerializeField]
        private CanvasManager _canvasManager;

        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void EnableCanvas()
        {
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(_canvasGroup, 1f, _canvasManager.AnimationDuration)
                .setEase(LeanTweenType.easeInOutQuad);
            LeanTween.scale(gameObject, Vector3.one * _canvasManager.EnableCanvasScale, _canvasManager.AnimationDuration)
                .setEase(LeanTweenType.easeInOutQuad);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
        
        public void DisableCanvas()
        {
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(_canvasGroup, 0f, _canvasManager.AnimationDuration)
                .setEase(LeanTweenType.easeInOutQuad);
            LeanTween.scale(gameObject, Vector3.one * _canvasManager.DisableCanvasScale, _canvasManager.AnimationDuration)
                .setEase(LeanTweenType.easeInOutQuad);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}

