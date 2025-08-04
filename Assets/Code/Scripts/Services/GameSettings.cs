//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using UnityEngine;

namespace XaviGames.Services
{
    [CreateAssetMenu(fileName = nameof(GameSettings), menuName = "Xavi Games/Services/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        private enum ConnectionType
        {
            WSS,
            DTLS,
            UDP
        }

        [field: Header("Unity Ads")]
        [field: SerializeField]
        public string AndroidGameId { get; private set; }

        [field: SerializeField]
        public string IOSGameId { get; private set; }

        [field: SerializeField]
        public bool TestMode { get; private set; }

        [field: SerializeField]
        public string InterstitialAndroidId { get; private set; }

        [field: SerializeField]
        public string InterstitialIOSId { get; private set; }

        [field: Header("Unity Host")]
        [SerializeField]
        private ConnectionType _connectionType;

        [field: SerializeField]
        public int MinPlayersInMatch { get; private set; }

        [field: SerializeField]
        public int MaxPlayersInMatch { get; private set; }

        [field: SerializeField]
        [field: Min(0)]
        public int MinutesMatchDuration { get; private set; } = 2;

        [field: SerializeField]
        [field: Min(0)]
        public float StartDelayAfterMinPlayers { get; private set; }

        [field: SerializeField]
        [field: Min(0)]
        public int MatchOverBannerDelay { get; private set; } = 5;

        [field: SerializeField]
        [field: Min(0)]
        public int MatchEndDelay { get; private set; } = 15;

        private void OnValidate()
        {
            MinPlayersInMatch = Mathf.Max(2, MinPlayersInMatch);

            if (MaxPlayersInMatch < MinPlayersInMatch)
            {
                MaxPlayersInMatch = MinPlayersInMatch;
            }
        }

        public string GetConnectionType()
        {
            if (_connectionType == ConnectionType.WSS)
            {
                return "wss";
            }
            else if (_connectionType == ConnectionType.DTLS)
            {
                return "dtls";
            }
            else
            {
                return "udp";
            }
        }
    }
}
