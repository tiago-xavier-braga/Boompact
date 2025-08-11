// Boompact (c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Manager;
using XaviGames.Services;
using XaviGames.Ui;

namespace XaviGames.Host
{
    public class MatchController : MonoBehaviour
    {
        [SerializeField] 
        private GameManager _gameManager;
        
        [SerializeField] 
        private CarSpawnController _carSpawnController;
        
        [SerializeField] 
        private TeamController _teamController;
        
        [SerializeField] 
        private GameSettings _gameSettings;

        public void StartMatch()
        {
            _carSpawnController.SpawnAllCars();
            _teamController.DistributeInitialBombs();
            _gameManager.SetGameStateServer(GameState.GameInProgress);
            StartCoroutine(StartCountdown());
        }

        private IEnumerator StartCountdown()
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                GameLogger.LogWarning("StartMatch called on client. Aborting.", LogCategory.Server);
                yield break;
            }

            if (_gameSettings == null)
            {
                GameLogger.LogWarning("MatchSettings is not set. Cannot start countdown.", LogCategory.Server);
                yield break;
            }

            int currentSeconds = _gameSettings.MinutesMatchDuration * 60;
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
            _gameManager.SetGameStateServer(GameState.GameEnded);
            StartCoroutine(ShowMatchOverBannerAsync());
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void UpdateCountdownClientRpc(int remainingSeconds)
        {
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;
            //CanvasManager.Instance.MatchHud.UpdateTimer(minutes, seconds);
        }

        private IEnumerator ShowMatchOverBannerAsync()
        {
            MatchUIController.Instance.MatchEndHandler.EnableMatchOverBannerRpc();
            int remainingSeconds = _gameSettings.MatchOverBannerDelay;
            while (remainingSeconds > 0)
            {
                yield return new WaitForSeconds(1f);
                remainingSeconds--;
            }
            MatchUIController.Instance.MatchEndHandler.DisableMatchOverBannerRpc();

            StartCoroutine(ShowCanvasResultsAsync());
        }

        private IEnumerator ShowCanvasResultsAsync()
        {
            SendResultForClients();

            int remainingSeconds = _gameSettings.MatchEndDelay;
            while (remainingSeconds > 0)
            {
                yield return new WaitForSeconds(1f);
                remainingSeconds--;
            }

            MatchUIController.Instance.MatchEndHandler.DisableCanvasResultRpc();
            StartMatch();
        }

        private void SendResultForClients()
        {
            foreach (ulong playerId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                bool isWinner = _teamController.NonBombOwners.Contains(playerId);

                MatchUIController.Instance.MatchEndHandler.SendPlayerResultRpc(
                    isWinner, MatchUIController.Instance.MatchEndHandler.RpcTarget.Single(playerId, RpcTargetUse.Temp));
            }
        }
    }
}
