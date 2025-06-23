// Boompact (c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using XaviEssencials.Runtime;

namespace XaviGames.Ui
{
    public class RoomConnectSelector : CanvasGroupController
    {
        [SerializeField]
        private GameObject _roomListItemPrefab;

        [SerializeField]
        private Transform _content;

        [SerializeField]
        private int _maxListCount = 25;

        [Header("Info")]
        private List<LobbyListEntry> _instantiatedRoomItems = new();

        private Coroutine _createRoomListCoroutine;

        public override void EnableCanvas()
        {
            base.EnableCanvas();
            _createRoomListCoroutine = StartCoroutine(CreateRoomListCoroutine());
        }

        public override void DisableCanvas()
        {
            base.DisableCanvas();
            
            if (_createRoomListCoroutine != null)
            {
                StopCoroutine(_createRoomListCoroutine);
                _createRoomListCoroutine = null;
            }

            foreach (var item in _instantiatedRoomItems)
            {
                Destroy(item.gameObject);
            }
            _instantiatedRoomItems.Clear();
        }

        public void UnselectedAllButton()
        {
            foreach (var item in _instantiatedRoomItems)
            {
                item.UnselectButton();
            }
        }

        private IEnumerator CreateRoomListCoroutine()
        {
            while (true)
            {
                UpdateRoomList();
                yield return new WaitForSeconds(5f);
            }
        }


        private async Task<List<Lobby>> GetAvailableLobbies()
        {
            try
            {
                var queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
                return queryResponse.Results;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"Lobby query failed: {e.Message}");
                return new List<Lobby>();
            }
        }

        private async void UpdateRoomList()
        {
            List<Lobby> lobbies = await GetAvailableLobbies();

            foreach (var lobby in lobbies)
            {
                if (_instantiatedRoomItems.Count >= _maxListCount)
                { 
                    break;
                }

                string currentRelayCode = lobby.Data.ContainsKey("joinCode") ? lobby.Data["joinCode"].Value : "Invalid code";

                if (_instantiatedRoomItems.Find(button => button.RelayCode == currentRelayCode))
                {
                    continue;
                }

                var lobbyGameObject = Instantiate(_roomListItemPrefab, Vector3.zero, Quaternion.identity);
                lobbyGameObject.transform.SetParent(_content, false);

                LobbyListEntry lobbyListEntry = lobbyGameObject.GetComponent<LobbyListEntry>();

                GameLogger.Log(lobbyListEntry, LogCategory.Client);
                
                lobbyListEntry.SetLobbyInfos(
                    currentRelayCode,
                    lobby.Players.Count,
                    lobby.MaxPlayers
                );

                _instantiatedRoomItems.Add(lobbyListEntry);
            }
        }
    }
}