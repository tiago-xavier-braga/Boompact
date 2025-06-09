//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Services;

namespace XaviGames.Host
{
    public class MatchController : MonoBehaviour
    {
        [SerializeField]
        private HostManager _serverManager;

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
            _serverManager.SetServerState(HostState.GameInProgress);
            StartCoroutine(StartCountdown());
        }

        private IEnumerator StartCountdown()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                GameLogger.LogWarning("StartMatch called on client. Aborting.", LogCategory.Server);
                yield return null;
            }

            if (_hostSettings is null)
            {
                GameLogger.LogWarning("MatchSettings is not set. Cannot start countdown.", LogCategory.Server);
                yield return null;
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
            _serverManager.SetServerState(HostState.GameEnded);
            ShowWinnersClientRpc();
            //_serverManager.ResetMatch();
        }

        [Rpc(SendTo.NotServer)]
        private void UpdateCountdownClientRpc(int remainingSeconds)
        {
            var minutes = remainingSeconds / 60;
            var secs = remainingSeconds % 60;
            //HudManager.Instance.SetMatchTimer($"{minutes:D2}:{secs:D2}");
        }

        [Rpc(SendTo.NotServer)]
        private void ShowWinnersClientRpc()
        {
            //MenuManager.Instance.ShowResults(_teamController.BombOwners);
        }
    }
}
