using System;
using UnityEngine;

namespace XaviGames.Services
{
    [Serializable]
    public enum ServiceType
    {
        Local = 0,
        Cloud = 1
    }

    [Serializable]
    public enum BuildType
    {
        Client = 0,
        Server = 1
    }

    [CreateAssetMenu(fileName = "ServicesSettings", menuName = "Xavi Games/Services/ServicesSettings")]
    public class ServicesSettings : ScriptableObject
    {
        [field: Header("Build Settings")]
        [field: SerializeField]
        public BuildType BuildType { get; private set; }

        [field: SerializeField]
        public ServiceType BuildServiceType { get; private set; }

        [field: SerializeField]
        public ServiceType ClientServiceType { get; private set; }


        [field: Header("Unity Matchmaker")]
        [field: SerializeField]
        public string QueueName { get; private set; }

        [field: Header("Tests Settings")]
        [field: SerializeField]
        public string TestServerIP { get; private set; } = "127.0.0.1";

        [field:SerializeField]
        public ushort TestServerPort { get; private set; } = 7777;
    }
}

