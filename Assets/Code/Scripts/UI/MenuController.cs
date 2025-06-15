//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using TMPro;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Host;
using XaviGames.Manager;

namespace XaviGames.Ui
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _joinCodeText;

        [SerializeField]
        private TMP_InputField _joinCodeInputField;

        [SerializeField]
        private CanvasManager _canvasManager;

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

            string joinCode = _joinCodeInputField.text.Trim();

            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogError("Join code cannot be empty.");
                return;
            }

            _canvasManager.LoadingCanvasController.EnableLoading();
            ClientManager.Instance.StartClientWithRelay(joinCode, HandleServiceResponse);
        }

        private void HandleServiceResponse(bool isSuccess)
        {
            if (isSuccess)
            {
                GameLogger.Log("Service successfully completed", LogCategory.Client);
            }
            else
            {
                _canvasManager.LoadingCanvasController.DisableLoading();
                GameLogger.LogError("Unable to access this service", LogCategory.Client);
            }
        }
    }
}
