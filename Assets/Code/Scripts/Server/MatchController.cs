//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Server
{
    public class MatchController : MonoBehaviour
    {
        [SerializeField]
        private ServerManager _serverManager;

        [SerializeField]
        private CarSpawnController _carSpawnController;

        [SerializeField]
        private TeamController _teamController;

        public void StartMatch()
        {
            _teamController.DividePlayersWithAndWithoutBombs();
            _carSpawnController.SpawnAllCars();
            _serverManager.SetServerState(ServerState.GameInProgress);
        }
    }

}
