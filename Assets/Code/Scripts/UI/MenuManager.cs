using TMPro;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Manager;
using XaviGames.Host;

namespace XaviGames.Ui
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _joinCodeText;

        [SerializeField]
        private TMP_InputField _joinCodeInputField;

        public async void StartHost()
        {
            if (HostManager.Instance is null)
            {
                GameLogger.LogError("HostManager instance is not initialized.", LogCategory.Client);
                return;
            }

            await LoadingCanvasController.Instance.EnableLoadingAsync();
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

            LoadingCanvasController.Instance.EnableLoading();
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
                LoadingCanvasController.Instance.DisableLoading();
                GameLogger.LogError("Unable to access this service", LogCategory.Client);
            }
        }
    }
}
