using UnityEngine;

namespace XaviGames.Ui
{
    public class MatchUIController : MonoBehaviour
    {
        [field: Header("Scripts References")]
        [field: SerializeField]
        public CanvasGroupController HudCanvasController { get; private set; }

        [field: SerializeField]
        public MatchEndHandler MatchEndHandler { get; private set; }

        public static MatchUIController Instance { get; private set; } = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
