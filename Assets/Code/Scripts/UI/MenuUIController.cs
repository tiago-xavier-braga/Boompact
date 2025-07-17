using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Host;
using XaviGames.Manager;

namespace XaviGames.Ui
{
    public class MenuUIController : MonoBehaviour
    {
        [field: Header("Scripts References")]
        [field: SerializeField]
        public RoomConnectSelector RoomConnectSelector { get; private set; }

        [field: Header("Info")]
        [field: SerializeField]
        [field: ReadOnly]
        public string JoinCode { get; private set; } = string.Empty;

        private LoadingCanvasController _loadingController;

        public static MenuUIController Instance { get; private set; } =  null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _loadingController = LoadingCanvasController.Instance;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public async void StartHost()
        {
            if (HostManager.Instance is null)
            {
                GameLogger.LogError("HostManager instance is not initialized.", LogCategory.Client);
                return;
            }

            await _loadingController.EnableLoadingAsync();
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

            _loadingController.EnableLoading();
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
            }
            else
            {
                _loadingController.DisableLoading();
                GameLogger.LogError("Unable to access this service", LogCategory.Client);
            }
        }
    }
}
