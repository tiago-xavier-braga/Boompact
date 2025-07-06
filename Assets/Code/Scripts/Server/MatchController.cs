// Boompact (c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Services;
using XaviGames.Ui;

namespace XaviGames.Host
{
    public class MatchController : MonoBehaviour
    {
        [SerializeField] 
        private HostManager _hostManager;
        
        [SerializeField] 
        private CarSpawnController _carSpawnController;
        
        [SerializeField] 
        private TeamController _teamController;
        
        [SerializeField] 
        private HostSettings _hostSettings;

        public void StartMatch()
        {
            _carSpawnController.SpawnAllCars();
            _teamController.DistributeInitialBombs();
            _hostManager.SetServerState(HostState.GameInProgress);
            StartCoroutine(StartCountdown());
        }

        private IEnumerator StartCountdown()
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                GameLogger.LogWarning("StartMatch called on client. Aborting.", LogCategory.Server);
                yield break;
            }

            if (_hostSettings == null)
            {
                GameLogger.LogWarning("MatchSettings is not set. Cannot start countdown.", LogCategory.Server);
                yield break;
            }

            int currentSeconds = _hostSettings.MinutesMatchDuration * 60;
            while (currentSeconds > 0)
            {
                currentSeconds--;
                UpdateCountdownClientRpc(currentSeconds);
                GameLogger.Log($"Match countdown: {currentSeconds} seconds remaining", LogCategory.Server);
                yield return new WaitForSeconds(1f);
            }

            FinishMatch();
        }

        private void FinishMatch()
        {
            _hostManager.SetServerState(HostState.GameEnded);
            StartCoroutine(ShowMatchOverBannerAsync());
        }

        [Rpc(SendTo.NotServer)]
        private void UpdateCountdownClientRpc(int remainingSeconds)
        {
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;
            //CanvasManager.Instance.MatchHud.UpdateTimer(minutes, seconds);
        }

        private IEnumerator ShowMatchOverBannerAsync()
        {
            CanvasManager canvasManager = CanvasManager.Instance;

            canvasManager.HudController.DisableCanvas();
            canvasManager.MatchEndHandler.EnableMatchOverBannerRpc();

            int remainingSeconds = _hostSettings.MatchOverBannerDelay;
            while (remainingSeconds > 0)
            {
                yield return new WaitForSeconds(1f);
                remainingSeconds--;
            }
            CanvasManager.Instance.MatchEndHandler.DisableMatchOverBannerRpc();

            StartCoroutine(ShowCanvasResultsAsync());
        }

        private IEnumerator ShowCanvasResultsAsync()
        {
            SendResultForClients();

            int remainingSeconds = _hostSettings.MatchEndDelay;
            while (remainingSeconds > 0)
            {
                yield return new WaitForSeconds(1f);
                remainingSeconds--;
            }

            CanvasManager.Instance.MatchEndHandler.DisableCanvasResultRpc();
            StartMatch();
        }

        private void SendResultForClients()
        {
            foreach (ulong playerId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                bool isWinner = _teamController.NonBombOwners.Contains(playerId);

                CanvasManager.Instance.MatchEndHandler.SendPlayerResultRpc(
                    isWinner, CanvasManager.Instance.MatchEndHandler.RpcTarget.Single(playerId, RpcTargetUse.Temp));
            }
        }
    }
}
