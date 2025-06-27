// Boompact (c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Ui
{
    public class MatchEndHandler : NetworkBehaviour
    {
        [SerializeField] 
        private CanvasGroupController _matchEndCanvas;
        
        [SerializeField] 
        private TextMeshProUGUI _winnerText;
        
        [SerializeField] 
        private TextMeshProUGUI _timeText;
        
        [SerializeField] 
        private int _resetDelaySeconds = 10;

        [Rpc(SendTo.SpecifiedInParams)]
        public void SendPlayerResultRpc(bool isWinner, RpcParams rpcParams = default)
        {
            _winnerText.text = isWinner ? "Win" : "Lose";
            _matchEndCanvas.EnableCanvas();
            StartCoroutine(CountdownSeconds());
        }

        private IEnumerator CountdownSeconds()
        {
            int remainingSeconds = _resetDelaySeconds;
            while (remainingSeconds > 0)
            {
                _timeText.text = $"Reset in {remainingSeconds} seconds...";
                yield return new WaitForSeconds(1f);
                remainingSeconds--;
            }
        }
    }
}
