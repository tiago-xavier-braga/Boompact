// Boompact (c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using XaviGames.Services;

namespace XaviGames.Ui
{
    public class MatchEndHandler : NetworkBehaviour
    {
        [SerializeField]
        private CanvasGroupController _hudController;

        [SerializeField] 
        private CanvasGroupController _matchEndCanvas;

        [SerializeField]
        private CanvasGroupController _matchOverBannerCanvas;

        [SerializeField] 
        private TextMeshProUGUI _winnerText;
        
        [SerializeField] 
        private TextMeshProUGUI _timeText;

        [SerializeField]
        private HostSettings _hostSettings;

        [Rpc(SendTo.SpecifiedInParams)]
        public void SendPlayerResultRpc(bool isWinner, RpcParams rpcParams = default)
        {
            _winnerText.text = isWinner ? "Win" : "Lose";
            _matchEndCanvas.EnableCanvas();
            StartCoroutine(CountdownSeconds());
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DisableCanvasResultRpc()
        {
            _hudController.EnableCanvas();
            _matchEndCanvas.DisableCanvas();
            _timeText.text = string.Empty;
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void EnableMatchOverBannerRpc()
        {
            _hudController.DisableCanvas();
            _matchOverBannerCanvas.EnableCanvas();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DisableMatchOverBannerRpc()
        {
            _matchOverBannerCanvas.DisableCanvas();
        }

        private IEnumerator CountdownSeconds()
        {
            int remainingSeconds = _hostSettings.MatchEndDelay;
            while (remainingSeconds > 0)
            {
                _timeText.text = $"Reset in {remainingSeconds} seconds...";
                yield return new WaitForSeconds(1f);
                remainingSeconds--;
            }
        }
    }
}
