//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using UnityEngine;

namespace XaviGames.Services
{
    [CreateAssetMenu(fileName = "MatchmakerSettings", menuName = "Xavi Games/Services/MatchmakerSettings")]
    public class MatchmakerSettings : ScriptableObject
    {
        [field: Header("Unity Matchmaker")]
        [field: SerializeField]
        public string QueueName { get; private set; }

        [field: SerializeField]
        public int MinPlayersInMatch { get; private set; }

        [field: SerializeField]
        public int MaxPlayersInMatch { get; private set; }

        [field: SerializeField]
        [field: Min(0)]
        public float StartDelayAfterMinPlayers { get; private set; }
    }
}
