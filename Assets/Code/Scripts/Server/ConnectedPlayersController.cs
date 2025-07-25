//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Manager;
using XaviGames.Services;

namespace XaviGames.Host
{
    public class ConnectedPlayersController : MonoBehaviour
    {
        [SerializeField]
        private HostSettings _hostSettings;

        [SerializeField]
        private HostManager _hostManager;

        [SerializeField]
        private GameManager _gameManager;

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

        public void UpdatePlayerCount()
        {
            GameState currentState = _gameManager.GameState;

            if (currentState != GameState.WaitingForPlayers)
            {
                return;
            }

            if (_connectedPlayers == _hostSettings.MaxPlayersInMatch)
            {
                if (_startMatchCountdownCoroutine != null)
                {
                    StopCoroutine(_startMatchCountdownCoroutine);
                    _startMatchCountdownCoroutine = null;
                }
                //_serverManager.StartMatch();

                GameLogger.Log($"Maximum players reached. Starting game. Players in match: {_connectedPlayers}", LogCategory.Matchmaker);
            }
            else if (_connectedPlayers >= _hostSettings.MinPlayersInMatch)
            {
                if (_startMatchCountdownCoroutine == null)
                {
                    _startMatchCountdownCoroutine = StartCoroutine(StartMatchCountdown());
                }
            }
            else
            {
                _gameManager.SetGameStateRpc(GameState.WaitingForPlayers);

                if (_startMatchCountdownCoroutine != null)
                {
                    StopCoroutine(_startMatchCountdownCoroutine);
                    _startMatchCountdownCoroutine = null;
                }

                GameLogger.Log($"Waiting for more players. Players in match: {_connectedPlayers}", LogCategory.Matchmaker);
            }
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

        private IEnumerator StartMatchCountdown()
        {
            GameLogger.Log($"Minimum players reached. Starting match in {_hostSettings.StartDelayAfterMinPlayers} seconds...", LogCategory.Matchmaker);

            yield return new WaitForSeconds(_hostSettings.StartDelayAfterMinPlayers);

            if (_connectedPlayers >= _hostSettings.MinPlayersInMatch)
            {
                GameLogger.Log("Countdown finished. Starting match.", LogCategory.Matchmaker);
                _hostManager.StartMatch();
            }
            else
            {
                GameLogger.Log("Countdown canceled. Not enough players.", LogCategory.Matchmaker);
            }

            _startMatchCountdownCoroutine = null;
        }
    }
}