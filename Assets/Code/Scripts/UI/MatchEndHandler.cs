//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Ui
{
    public class MatchEndHandler : NetworkBehaviour
    {
        [Header("Match Over Banner")]
        [SerializeField]
        private CanvasGroupController _matchOverBanner;

        [SerializeField]
        private int _matchOverBannerDuration = 5;

        [Header("Match End Banner")]
        [SerializeField]
        private CanvasGroupController _matchEndCanvas;

        [SerializeField]
        private TextMeshProUGUI _winnerText;

        [SerializeField]
        private TextMeshProUGUI _timeText;

        [SerializeField]
        private int _matchEndBannerDuration = 15;

        public async Task ShowOverMatch()
        {
            _matchOverBanner.EnableCanvas();
            await Task.Delay(TimeSpan.FromSeconds(_matchOverBannerDuration));
            _matchOverBanner.DisableCanvas();
        }

        public async Task ShowWinnerMatch(List<ulong> bombPlayers)
        {
            if (bombPlayers.Contains(OwnerClientId))
            {
                _winnerText.text = "Lose";
            }
            else
            {
                _winnerText.text = "Win";
            }

            _matchEndCanvas.EnableCanvas();
            StartCoroutine(StartCountdown());
            await Task.Delay(TimeSpan.FromSeconds(_matchEndBannerDuration));
            _matchEndCanvas.DisableCanvas();
        }

        private IEnumerator StartCountdown()
        {
            int currentSeconds = _matchEndBannerDuration;
            while (currentSeconds > 0)
            {
                currentSeconds--;
                _timeText.text = $"Reset in {currentSeconds} seconds...";
                GameLogger.Log($"Match countdown: {currentSeconds} seconds remaining", LogCategory.Server);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
