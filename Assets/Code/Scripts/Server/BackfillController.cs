//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

#if UNITY_SERVER
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
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

        private string _ticketId = string.Empty;

        public async Task CreateBackfillTicket()
        {
            var serviceType = _servicesSettings.BuildServiceType;
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

            GameLogger.Log($"Environment: {results.EnvironmentId} MatchId: {results.MatchId}" +
                $" MatchProperties: {results.MatchProperties}", LogCategory.Matchmaker);

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

            GameLogger.Log("Requesting backfill ticket", LogCategory.Matchmaker);
            _ticketId = await MatchmakerService.Instance.CreateBackfillTicketAsync(options);
        }

        public async Task ApproveBackfillTicketEverySecond()
        {
            const int delayBeforeStart = 5;

            for (int i = delayBeforeStart - 1; i >= 0; i--)
            {
                GameLogger.Log($"Waiting {i} seconds to start backfill", LogCategory.Matchmaker);
                await Task.Delay(1000);
            }

            while (true)
            {
                await Task.Delay(1000);

                if (string.IsNullOrWhiteSpace(_ticketId))
                {
                    GameLogger.Log("No backfill ticket to approve", LogCategory.Matchmaker);
                    continue;
                }

                GameLogger.Log($"Attempting backfill approval for ticket: {_ticketId}", LogCategory.Matchmaker);

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