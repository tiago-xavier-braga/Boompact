using UnityEngine;

namespace XaviGames.Services
{
    [CreateAssetMenu(fileName = "ServicesSettings", menuName = "Xavi Games/ServicesSettings")]
    public class ServicesSettings : ScriptableObject
    {
        [field: Header("Unity Matchmaker")]
        [field: SerializeField]
        public string QueueName { get; private set; }
    }
}

