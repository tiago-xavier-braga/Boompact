// Boompact (c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Car;
using XaviGames.Manager;
using XaviGames.PlayerSystem;

namespace XaviGames.Host
{
    public class TeamController : MonoBehaviour
    {
        [SerializeField]
        private GameManager _gameManager;

        public List<ulong> BombOwners { get; private set; } = new();
        public List<ulong> NonBombOwners { get; private set; } = new();
        private readonly List<ulong> _connectedPlayerIds = new();

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
            }
        }

        public void DistributeInitialBombs()
        {
            BombOwners.Clear();
            NonBombOwners.Clear();

            var shuffledIds = _connectedPlayerIds.OrderBy(_ => Random.value).ToList();
            int halfCount = shuffledIds.Count / 2;

            BombOwners.AddRange(shuffledIds.Take(halfCount));
            NonBombOwners.AddRange(shuffledIds.Skip(halfCount));

            foreach (var clientId in BombOwners)
            {
                var bombHandler = GetCarBombHandler(clientId);
                if (bombHandler != null)
                {
                    bombHandler.GiveBombRpc();
                }
            }

            GameLogger.Log($"Bombs distributed. Owners: {BombOwners.Count}, Non-owners: {NonBombOwners.Count}", LogCategory.Server);
        }

        public void TransferBombBetweenPlayers(ulong fromClientId, ulong toClientId)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                GameLogger.LogWarning("TransferBombBetweenPlayers can only be called on the server.", LogCategory.Server);
                return;
            }

            if (_gameManager.GameState != GameState.GameInProgress)
            {
                return;
            }

            if (fromClientId == toClientId
                || !BombOwners.Contains(fromClientId)
                || BombOwners.Contains(toClientId))
            {
                GameLogger.LogWarning($"Invalid bomb transfer request from {fromClientId} to {toClientId}.", LogCategory.Server);
                return;
            }

            BombOwners.Remove(fromClientId);
            NonBombOwners.Add(fromClientId);

            BombOwners.Add(toClientId);
            NonBombOwners.Remove(toClientId);

            var fromHandler = GetCarBombHandler(fromClientId);
            fromHandler?.RemoveBombRpc();

            var toHandler = GetCarBombHandler(toClientId);
            toHandler?.GiveBombRpc();

            GameLogger.Log($"Bomb transferred from {fromClientId} to {toClientId}.", LogCategory.Server);
        }

        private CarBombHandler GetCarBombHandler(ulong clientId)
        {
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var netClient))
            {
                GameLogger.LogWarning($"Client {clientId} not found in ConnectedClients.", LogCategory.Server);
                return null;
            }

            var playerObject = netClient.PlayerObject;
            if (playerObject == null)
            {
                GameLogger.LogWarning($"PlayerObject is null for client {clientId}.", LogCategory.Server);
                return null;
            }

            var playerController = playerObject.GetComponent<PlayerController>();
            if (playerController == null || playerController.CarSpawned == null)
            {
                GameLogger.LogWarning($"PlayerController or CarSpawned is null for client {clientId}.", LogCategory.Server);
                return null;
            }

            var bombHandler = playerController.CarSpawned.GetComponent<CarBombHandler>();
            if (bombHandler == null)
            {
                GameLogger.LogWarning($"CarBombHandler not found on CarSpawned for client {clientId}.", LogCategory.Server);
                return null;
            }

            return bombHandler;
        }

        private void HandleClientConnected(ulong clientId)
        {
            if (_connectedPlayerIds.Contains(clientId))
            {
                GameLogger.LogWarning($"Client {clientId} is already connected.", LogCategory.Server);
                return;
            }

            _connectedPlayerIds.Add(clientId);
        }

        private void HandleClientDisconnected(ulong clientId)
        {
            _connectedPlayerIds.RemoveAll(id => id == clientId);
            BombOwners.RemoveAll(id => id == clientId);
            NonBombOwners.RemoveAll(id => id == clientId);
        }
    }
}
