//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using UnityEngine;

namespace XaviGames.Ui
{
    public class CanvasManager : MonoBehaviour
    {
        [field: SerializeField]
        public MenuController MenuController { get; private set; }

        [field: SerializeField]
        public LoadingCanvasController LoadingCanvasController { get; private set; }

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

    }
}
