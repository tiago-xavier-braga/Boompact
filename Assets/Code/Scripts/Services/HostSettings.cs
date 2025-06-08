//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using UnityEngine;

namespace XaviGames.Services
{
    [CreateAssetMenu(fileName = nameof(HostSettings), menuName = "Xavi Games/Services/Host Settings")]
    public class HostSettings : ScriptableObject
    {
        [field: Header("Unity Host")]

        [field: SerializeField]
        public int MinPlayersInMatch { get; private set; }

        [field: SerializeField]
        public int MaxPlayersInMatch { get; private set; }

        [field: SerializeField]
        [field: Min(0)]
        public float StartDelayAfterMinPlayers { get; private set; }


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
#if UNITY_WEBGL
    return "wss";
#elif UNITY_ANDROID || UNITY_IOS
    return "dtls";
#else
            return "udp";
#endif
        }
    }
}
