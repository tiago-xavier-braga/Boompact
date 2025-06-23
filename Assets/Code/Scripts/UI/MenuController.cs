//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Host;
using XaviGames.Manager;

namespace XaviGames.Ui
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private CanvasManager _canvasManager;

        [field: Header("Info")]
        [field: SerializeField]
        [field: ReadOnly]
        public string JoinCode { get; private set; } = string.Empty;

        public async void StartHost()
        {
            if (HostManager.Instance is null)
            {
                GameLogger.LogError("HostManager instance is not initialized.", LogCategory.Client);
                return;
            }

            await _canvasManager.LoadingCanvasController.EnableLoadingAsync();
            HostManager.Instance.StartHostWithRelay(HandleServiceResponse);
        }

        public void JoinGame()
        {
            if (ClientManager.Instance is null)
            {
                GameLogger.LogError("ClientManager instance is not initialized.", LogCategory.Client);
                return;
            }

            string joinCode = JoinCode.Trim();

            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogError("Join code cannot be empty.");
                return;
            }

            _canvasManager.LoadingCanvasController.EnableLoading();
            ClientManager.Instance.StartClientWithRelay(joinCode, HandleServiceResponse);
        }

        public void SetJoinCode(string joinCode)
        {
            if (string.IsNullOrEmpty(joinCode))
            {
                GameLogger.LogError("Join code cannot be empty.", LogCategory.Client);
                return;
            }

            JoinCode = joinCode.Trim();
            GameLogger.Log($"Join code set to: {JoinCode}", LogCategory.Client);
        }

        private void HandleServiceResponse(bool isSuccess)
        {
            if (isSuccess)
            {
                GameLogger.Log("Service successfully completed", LogCategory.Client);
                //_canvasManager.LoadingCanvasController.DisableLoading();
            }
            else
            {
                //_canvasManager.LoadingCanvasController.DisableLoading();
                GameLogger.LogError("Unable to access this service", LogCategory.Client);
            }
        }
    }
}
