using UnityEngine;

namespace XaviGames.UI
{
    [CreateAssetMenu(fileName = "CanvasTransitionSO", menuName = "Xavi Games/UI/CanvasTransitionSO")]
    public class CanvasTransitionSO : ScriptableObject
    {
        [field: Header("Canvas Group Controller")]
        [field: SerializeField]
        public float EnableCanvasScale { get; private set; } = 1f;

        [field: SerializeField]
        public float DisableCanvasScale { get; private set; } = 0.8f;

        [field: SerializeField]
        public float AnimationDuration { get; private set; } = 0.5f;
    }
}
