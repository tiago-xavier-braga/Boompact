//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Car;
using XaviGames.PlayerSystem;

namespace XaviGames.Server
{
    public class TeamController : MonoBehaviour
    {
        public List<ulong> PlayersWithBomb { get; private set; } = new();
        public List<ulong> PlayersWithoutBomb { get; private set; } = new();

        private readonly List<ulong> _connectedPlayers = new();

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            if (_connectedPlayers.Contains(clientId))
            {
                GameLogger.LogWarning($"Client {clientId} is already connected.", LogCategory.Server);
                return;
            }

            _connectedPlayers.Add(clientId);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            _connectedPlayers.RemoveAll(c => c == clientId);
            PlayersWithBomb.RemoveAll(c => c == clientId);
            PlayersWithoutBomb.RemoveAll(c => c == clientId);
        }

        public void DividePlayersWithAndWithoutBombs()
        {
            PlayersWithBomb.Clear();
            PlayersWithoutBomb.Clear();

            var shuffledPlayers = new List<ulong>(_connectedPlayers);
            ShuffleList(shuffledPlayers);

            int half = shuffledPlayers.Count / 2;

            PlayersWithBomb.AddRange(shuffledPlayers.Take(half));
            PlayersWithoutBomb.AddRange(shuffledPlayers.Skip(half));

            foreach (var clientId in PlayersWithBomb)
            {
                if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
                {
                    GameLogger.LogWarning($"Client {clientId} not found in connected list.", LogCategory.Server);
                    continue;
                }

                var playerObj = networkClient.PlayerObject;
                if (playerObj == null)
                {
                    GameLogger.LogWarning($"PlayerObject for client {clientId} is null.", LogCategory.Server);
                    continue;
                }

                var playerController = playerObj.GetComponent<PlayerController>();
                if (playerController == null)
                {
                    GameLogger.LogWarning($"PlayerController component not found on client {clientId}.", LogCategory.Server);
                    continue;
                }

                var carGO = playerController.CarSpawned;
                if (carGO == null)
                {
                    GameLogger.LogWarning($"CarSpawned for player {clientId} is null. Cannot give bomb.", LogCategory.Server);
                    continue;
                }

                var bombHandler = carGO.GetComponent<CarBombHandler>();
                if (bombHandler == null)
                {
                    GameLogger.LogWarning($"CarBombHandler not found on {carGO.name} (client {clientId}).", LogCategory.Server);
                    continue;
                }

                bombHandler.GiveBomb();
            }

            GameLogger.Log($"Assigned bombs. Players with bombs: {PlayersWithBomb.Count}, without bomb: {PlayersWithoutBomb.Count}", LogCategory.Server);
        }

        public void TransferBomb(ulong clientFrom, ulong clientTo)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                GameLogger.LogWarning("TransferBomb só pode ser chamado no servidor.", LogCategory.Server);
                return;
            }

            if (clientFrom == clientTo)
            {
                GameLogger.LogWarning("Invalid transfer request: mesma pessoa.", LogCategory.Server);
                return;
            }

            if (!PlayersWithBomb.Contains(clientFrom) || PlayersWithBomb.Contains(clientTo))
            {
                GameLogger.LogWarning($"Transfer inválido de {clientFrom} para {clientTo}.", LogCategory.Server);
                return;
            }

            PlayersWithBomb.Remove(clientFrom);
            PlayersWithoutBomb.Add(clientFrom);

            PlayersWithBomb.Add(clientTo);
            PlayersWithoutBomb.Remove(clientTo);

            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientFrom, out var fromNetClient))
            {
                var fromObj = fromNetClient.PlayerObject;
                if (fromObj != null)
                {
                    var fromPC = fromObj.GetComponent<PlayerController>();
                    if (fromPC != null && fromPC.CarSpawned != null)
                    {
                        var fromBombHandler = fromPC.CarSpawned.GetComponent<CarBombHandler>();
                        if (fromBombHandler != null)
                            fromBombHandler.RemoveBomb();
                    }
                }
            }

            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientTo, out var toNetClient))
            {
                var toObj = toNetClient.PlayerObject;
                if (toObj != null)
                {
                    var toPC = toObj.GetComponent<PlayerController>();
                    if (toPC != null && toPC.CarSpawned != null)
                    {
                        var toBombHandler = toPC.CarSpawned.GetComponent<CarBombHandler>();
                        if (toBombHandler != null)
                            toBombHandler.GiveBomb();
                    }
                }
            }

            GameLogger.Log($"Bomb transferred from {clientFrom} to {clientTo}.", LogCategory.Server);
        }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
    }
}