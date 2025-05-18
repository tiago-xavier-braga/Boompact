//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System;
using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Server
{
    public class MatchController : MonoBehaviour
    {
        [field: SerializeField]
        public ServerManager ServerManager { get; private set; }

        public void OnListeningServerState(ServerState state)
        {
            if (state == ServerState.StartingGame)
            {
                GameLogger.Log("Starting match...", LogCategory.Server);
                StartMatch();
            }
        }

        private void StartMatch()
        {
            throw new NotImplementedException();
        }
    }
}
