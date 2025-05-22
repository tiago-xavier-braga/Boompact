//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace XaviGames.Server
{
    public class MatchController : MonoBehaviour
    {
        [SerializeField]
        private ServerManager _serverManager;

        private readonly List<NetworkClient> _connectedPlayers = new();
        private readonly List<NetworkClient> _playersWithBombs = new();
        private readonly List<NetworkClient> _playersWithoutBombs = new();

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void OnDisable()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        public void StartMatch()
        {
            //var state = _serverManager.ServerState;
            //if (state != ServerState.StartingGame)
            //{
            //    Debug.LogWarning("The match has already started");
            //    return;
            //}

            DividePlayersWithAndWithoutBombs();

            Debug.Log($"Assigned pumps. Players with bombs:{_playersWithBombs.Count}, without bomb: {_playersWithoutBombs.Count}");
        
            _serverManager.SetServerState(ServerState.GameInProgress);
        }

        private void DividePlayersWithAndWithoutBombs()
        {
            _playersWithBombs.Clear();
            _playersWithoutBombs.Clear();

            var shuffledPlayers = new List<NetworkClient>(_connectedPlayers);
            ShuffleList(shuffledPlayers);

            int half = shuffledPlayers.Count / 2;

            _playersWithBombs.AddRange(shuffledPlayers.Take(half));
            _playersWithoutBombs.AddRange(shuffledPlayers.Skip(half));
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
            _connectedPlayers.Add(client);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            _connectedPlayers.RemoveAll(c => c.ClientId == clientId);
        }
    }

}
