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
using System.Threading;

namespace XaviGames.Server
{
    public class BackfillController : MonoBehaviour
    {
        [SerializeField]
        private ServicesSettings _servicesSettings;

        [SerializeField]
        private MatchmakerSettings _matchmakerSettings;

        [SerializeField]
        private ServerManager _serverManager;

        [SerializeField]
        [ReadOnly]
        private string _ticketId = string.Empty;

        private CancellationTokenSource _backfillCancellationTokenSource;

        public async Task CreateBackfillTicket()
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
                _matchmakerSettings.QueueName,
                connectionString,
                new Dictionary<string, object>(),
                backfillTicketProperties
            );

            try
            {
                _ticketId = await MatchmakerService.Instance.CreateBackfillTicketAsync(options);
                _serverManager.SetServerState(ServerState.WaitingForPlayers);

                _backfillCancellationTokenSource?.Cancel();
                _backfillCancellationTokenSource = new CancellationTokenSource();
                await ApproveBackfillTicketEverySecond(_backfillCancellationTokenSource.Token);

            }
            catch (Exception ex)
            {
                GameLogger.LogError($"Failed to create backfill ticket: {ex.Message}", LogCategory.Matchmaker);
                return;
            }
        }

        public async Task DeleteBackfillTicket()
        {
            if (string.IsNullOrWhiteSpace(_ticketId))
            {
                GameLogger.Log("No backfill ticket to delete", LogCategory.Matchmaker);
                return;
            }
            try
            {
                if (_backfillCancellationTokenSource != null)
                {
                    _backfillCancellationTokenSource.Cancel();
                    _backfillCancellationTokenSource.Dispose();
                    _backfillCancellationTokenSource = null;
                }

                await MatchmakerService.Instance.DeleteTicketAsync(_ticketId);
                GameLogger.Log($"Deleted backfill ticket: {_ticketId}", LogCategory.Matchmaker);
            }
            catch (Exception ex)
            {
                GameLogger.LogError($"Failed to delete backfill ticket: {_ticketId}." +
                    $"Error: {ex.Message}", LogCategory.Matchmaker);
            }
        }

        private async Task ApproveBackfillTicketEverySecond(CancellationToken token)
        {
            const int delayBeforeStart = 5;

            GameLogger.Log($"Waiting {delayBeforeStart} seconds to start backfill", LogCategory.Matchmaker);
            await Task.Delay(TimeSpan.FromSeconds(delayBeforeStart), token);

            while (!token.IsCancellationRequested && (_serverManager.ServerState == ServerState.WaitingForPlayers))
            {
                await Task.Delay(1000);

                if (string.IsNullOrWhiteSpace(_ticketId))
                {
                    GameLogger.Log("No backfill ticket to approve", LogCategory.Matchmaker);
                    continue;
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