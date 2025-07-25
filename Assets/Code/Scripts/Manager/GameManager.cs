// Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com


using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Host;

namespace XaviGames.Manager
{
    public class GameManager : NetworkBehaviour
    {
        [field: SerializeField]
        [field: ReadOnly]
        public GameState GameState { get; private set; } = GameState.Off;

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

        [Rpc(SendTo.ClientsAndHost)]
        public void SetGameStateRpc(GameState state)
        {
            GameState = state;
            GameLogger.Log($"Server state changed to: {state}", LogCategory.Server);
        }
    }
}
