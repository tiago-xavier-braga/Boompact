//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Services;

namespace XaviGames.Server
{
    public class ConnectedPlayersController : MonoBehaviour
    {
        [SerializeField]
        private MatchmakerSettings _matchmakerSettings;

        [SerializeField]
        private ServerManager _serverManager;

        [SerializeField]
        [ReadOnly]
        private int _connectedPlayers => NetworkManager.Singleton.ConnectedClientsList.Count;

        private Coroutine _startMatchCountdownCoroutine = null;

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

        private void OnClientConnected(ulong clientId)
        {
            UpdatePlayerCount();
            GameLogger.Log($"Player connected. Players in match: {_connectedPlayers}", LogCategory.Matchmaker);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            UpdatePlayerCount();
            GameLogger.Log($"Player disconnected. Players in match: {_connectedPlayers}", LogCategory.Matchmaker);
        }

        private void UpdatePlayerCount()
        {
            ServerState currentState = _serverManager.ServerState;

            if (currentState != ServerState.WaitingForPlayers)
            {
                return;
            }

            //_connectedPlayers = NetworkManager.Singleton.ConnectedClientsList.Count;

            if (_connectedPlayers == _matchmakerSettings.MaxPlayersInMatch)
            {
                _serverManager.SetServerState(ServerState.StartingGame);

                if (_startMatchCountdownCoroutine != null)
                {
                    StopCoroutine(_startMatchCountdownCoroutine);
                    _startMatchCountdownCoroutine = null;
                }

                GameLogger.Log($"Maximum players reached. Starting game. Players in match: {_connectedPlayers}", LogCategory.Matchmaker);
            }
            else if (_connectedPlayers >= _matchmakerSettings.MinPlayersInMatch)
            {
                if (_startMatchCountdownCoroutine == null)
                {
                    _startMatchCountdownCoroutine = StartCoroutine(StartMatchCountdown());
                }
            }
            else
            {
                _serverManager.SetServerState(ServerState.WaitingForPlayers);

                if (_startMatchCountdownCoroutine != null)
                {
                    StopCoroutine(_startMatchCountdownCoroutine);
                    _startMatchCountdownCoroutine = null;
                }

                GameLogger.Log($"Waiting for more players. Players in match: {_connectedPlayers}", LogCategory.Matchmaker);
            }
        }

        private IEnumerator StartMatchCountdown()
        {
            GameLogger.Log($"Minimum players reached. Starting match in {_matchmakerSettings.StartDelayAfterMinPlayers} seconds...", LogCategory.Matchmaker);

            yield return new WaitForSeconds(_matchmakerSettings.StartDelayAfterMinPlayers);

            if (_connectedPlayers >= _matchmakerSettings.MinPlayersInMatch)
            {
                GameLogger.Log("Countdown finished. Starting match.", LogCategory.Matchmaker);
                _serverManager.StartMatch();
            }
            else
            {
                GameLogger.Log("Countdown canceled. Not enough players.", LogCategory.Matchmaker);
            }

            _startMatchCountdownCoroutine = null;
        }
    }
}