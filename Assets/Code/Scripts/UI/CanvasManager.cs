//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections.Generic;
using UnityEngine;

namespace XaviGames.Ui
{
    public class CanvasManager : MonoBehaviour
    {
        [field: SerializeField]
        public MenuController MenuController { get; private set; }

        [field: SerializeField]
        public LoadingCanvasController LoadingCanvasController { get; private set; }

        [field: SerializeField]
        public RoomConnectSelector RoomConnectSelector { get; private set; }

        [field: SerializeField]
        public MatchEndHandler MatchEndHandler { get; private set; }

        [field: SerializeField]
        public CanvasGroupController HudCanvasController { get; private set; }

        [field: Header("Canvas Group Controller")]
        [field: SerializeField]
        public float EnableCanvasScale { get; private set; } = 1f;

        [field: SerializeField]
        public float DisableCanvasScale { get; private set; } = 0.8f;

        [field: SerializeField]
        public float AnimationDuration { get; private set; } = 0.5f;

        [field: Space]
        [field: SerializeField]
        public List<CanvasGroupController> CanvasGroupControllerRefs { get; private set; } = new();

        public static CanvasManager Instance { get; private set; } = null;

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

        public void DisableAllCanvasGroups()
        {
            foreach (var controller in CanvasGroupControllerRefs)
            {
                controller.InstantDisableCanvas();
            }
        }

    }
}
