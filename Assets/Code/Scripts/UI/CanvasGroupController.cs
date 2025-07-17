using UnityEngine;
using UnityEngine.Events;
using XaviEssencials.Runtime;
using XaviGames.UI;

namespace XaviGames.Ui
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupController : MonoBehaviour
    {
        [SerializeField]
        private CanvasTransitionSO _canvasTransitionSO;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        public virtual void EnableCanvas()
        {
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(_canvasGroup, 1f, _canvasTransitionSO.AnimationDuration)
                .setEase(LeanTweenType.easeInOutQuad);
            LeanTween.scale(gameObject, Vector3.one * _canvasTransitionSO.EnableCanvasScale, _canvasTransitionSO.AnimationDuration)
                .setEase(LeanTweenType.easeInOutQuad);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
        
        public virtual void DisableCanvas()
        {
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(_canvasGroup, 0f, _canvasTransitionSO.AnimationDuration)
                .setEase(LeanTweenType.easeInOutQuad);
            LeanTween.scale(gameObject, Vector3.one * _canvasTransitionSO.DisableCanvasScale, _canvasTransitionSO.AnimationDuration)
                .setEase(LeanTweenType.easeInOutQuad);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void InstantDisableCanvas()
        {
            LeanTween.cancel(gameObject);
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}

