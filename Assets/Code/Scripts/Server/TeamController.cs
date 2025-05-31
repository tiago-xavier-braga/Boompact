//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Server
{
    public class TeamController : MonoBehaviour
    {
        public List<NetworkClient> PlayersWithBomb { get; private set; } = new();
        public List<NetworkClient> PlayersWithoutBomb { get; private set; } = new();
        
        private readonly List<NetworkClient> _connectedPlayers = new();

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void OnDestroy()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }


        public void DividePlayersWithAndWithoutBombs()
        {
            PlayersWithBomb.Clear();
            PlayersWithoutBomb.Clear();

            var shuffledPlayers = new List<NetworkClient>(_connectedPlayers);
            ShuffleList(shuffledPlayers);

            int half = shuffledPlayers.Count / 2;

            PlayersWithBomb.AddRange(shuffledPlayers.Take(half));
            PlayersWithoutBomb.AddRange(shuffledPlayers.Skip(half));

            GameLogger.Log($"Assigned pumps. Players with bombs:{PlayersWithBomb.Count}, without bomb: {PlayersWithoutBomb.Count}", LogCategory.Server);
        }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            var client = NetworkManager.Singleton.ConnectedClients[clientId];

            if (_connectedPlayers.Any(c => c.ClientId == clientId))
            {
                GameLogger.LogWarning($"Client {clientId} is already connected.", LogCategory.Server);
                return;
            }

            _connectedPlayers.Add(client);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            _connectedPlayers.RemoveAll(c => c.ClientId == clientId);
        }
    }
}