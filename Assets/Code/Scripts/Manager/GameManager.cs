// Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com


using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Manager
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private int _MaxFps = 120;

        public static GameManager Instance { get; private set; } = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _MaxFps;
        }
    }
}
