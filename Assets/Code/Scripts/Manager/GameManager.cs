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
        public static GameManager Instance { get; private set; }

        public readonly NetworkVariable<GameState> NetState =
            new NetworkVariable<GameState>(
                GameState.Off,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server);

        public GameState GameState => NetState.Value;

        [SerializeField]
        [ReadOnly] 
        private int _maxFps = 120;

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
            Application.targetFrameRate = _maxFps;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetState.Value = GameState.Off;
            }

            NetState.OnValueChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            NetState.OnValueChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState prev, GameState next)
        {
            GameLogger.Log($"State changed: {prev} -> {next}", LogCategory.Server);
        }

        public void SetGameStateServer(GameState newState)
        {
            if (!IsServer)
            {
                return;
            }
            
            NetState.Value = newState;
        }
    }

}
