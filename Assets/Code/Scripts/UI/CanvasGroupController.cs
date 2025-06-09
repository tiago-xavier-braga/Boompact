using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Ui
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupController : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private float _enableCanvasScale = 1f;

        [SerializeField]
        [ReadOnly]
        private float _disableCanvasScale = 0.8f;

        [SerializeField]
        [ReadOnly]
        private float _duration = 0.5f;

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
            LeanTween.alphaCanvas(_canvasGroup, 1f, _duration).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.scale(gameObject, Vector3.one * _enableCanvasScale, _duration).setEase(LeanTweenType.easeInOutQuad);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
        
        public void DisableCanvas()
        {
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(_canvasGroup, 0f, _duration).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.scale(gameObject, Vector3.one * _disableCanvasScale, _duration).setEase(LeanTweenType.easeInOutQuad);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}

