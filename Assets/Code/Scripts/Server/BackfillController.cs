//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

#if UNITY_SERVER
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using XaviEssencials.Runtime;
using Unity.Services.Multiplay;
using XaviGames.Services;

namespace XaviGames.Server
{
    public class BackfillController : MonoBehaviour
    {
        [SerializeField]
        private ServicesSettings _servicesSettings;

        [SerializeField]
        private ServerManager _serverManager;

        private string _ticketId = string.Empty;

        private int _currentPlayers => _serverManager.PlayersCount;
        private int _minPlayers => _servicesSettings.MinPlayers;
        private int _maxPlayers => _servicesSettings.MaxPlayers;

        public async Task<bool> CreateBackfillTicket()
        {
            var serviceType = _servicesSettings.ServerServiceType;
            var results = new MatchmakingResults();

            if (serviceType == ServiceType.Local)
            {
                string json = File.ReadAllText(Path.Combine(Application.streamingAssetsPath,
                    "MockMultiplayPayload.json"));
                results = JsonUtility.FromJson<MatchmakingResults>(json);
            }
            else
            {
                results = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();
            }

            var backfillTicketProperties = new BackfillTicketProperties(results.MatchProperties);

            string connectionString = serviceType == ServiceType.Local
                ? $"{_servicesSettings.TestServerIP}:{_servicesSettings.TestServerPort}"
                : MultiplayService.Instance.ServerConfig.IpAddress + ":" + MultiplayService.Instance.ServerConfig.Port;

            var options = new CreateBackfillTicketOptions(
                _servicesSettings.QueueName,
                connectionString,
                new Dictionary<string, object>(),
                backfillTicketProperties
            );

            try
            {
                _ticketId = await MatchmakerService.Instance.CreateBackfillTicketAsync(options);
                _serverManager.SetServerState(ServerState.WaitingForPlayers);
                return true;
            }
            catch (Exception ex)
            {
                GameLogger.LogError($"Failed to create backfill ticket: {ex.Message}", LogCategory.Matchmaker);
                return false;
            }
        }

        public async Task ApproveBackfillTicketEverySecond()
        {
            const int delayBeforeStart = 5;

            GameLogger.Log($"Waiting {delayBeforeStart} seconds to start backfill", LogCategory.Matchmaker);
            await Task.Delay(TimeSpan.FromSeconds(delayBeforeStart));

            while (_serverManager.ServerState == ServerState.WaitingForPlayers ||
                _serverManager.ServerState == ServerState.RestartingGame)
            {
                await Task.Delay(1000);

                if (string.IsNullOrWhiteSpace(_ticketId))
                {
                    GameLogger.Log("No backfill ticket to approve", LogCategory.Matchmaker);
                    continue;
                }

                if (_currentPlayers >= _maxPlayers)
                {
                    GameLogger.Log("Max players reached. No more backfill approvals.", LogCategory.Matchmaker);
                    
                    try
                    {
                        await MatchmakerService.Instance.DeleteBackfillTicketAsync(_ticketId);
                        GameLogger.Log("Backfill ticket deleted as max players reached.", LogCategory.Matchmaker);
                    }
                    catch (Exception ex)
                    {
                        GameLogger.LogError($"Failed to delete backfill ticket: {ex.Message}", LogCategory.Matchmaker);
                    }

                    break;
                }
                try
                {
                    var approvalOperation = await MatchmakerService.Instance.ApproveBackfillTicketAsync(_ticketId);
                    GameLogger.Log($"Approved backfill ticket: {_ticketId}", LogCategory.Matchmaker);
                }
                catch (Exception ex)
                {
                    GameLogger.LogError($"Failed to approve backfill ticket: {_ticketId}." +
                        $"Error: {ex.Message}", LogCategory.Matchmaker);
                }
            }
        }
    }
}
#endif