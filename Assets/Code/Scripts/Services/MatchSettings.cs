//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using UnityEngine;

namespace XaviGames.Services
{
    [CreateAssetMenu(fileName = "MatchSettings", menuName = "Xavi Games/Services/MatchSettings")]
    public class MatchSettings : ScriptableObject
    {
        [field: Header("Match Settings")]
        [field: SerializeField]
        public int MinutesMatchDuration { get; private set; } = 2;
    }
}
