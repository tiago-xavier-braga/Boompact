using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using XaviEssencials.Runtime;

namespace XaviGames.Manager
{
    public class StartupManager : MonoBehaviour
    {
        [field: SerializeField]
        private SceneBundle _clientSceneBundle;


        [field: SerializeField]
        private SceneBundle _serverSceneBundle;

        [Header("Build Settings")]
        [SerializeField]
        private bool _isClientBuild;

        private async void Start()
        {
            if (_isClientBuild)
            {
                await LoadScenesFromBundleAsync(_clientSceneBundle);
            }
            else
            {
                await LoadScenesFromBundleAsync(_serverSceneBundle);

            }
        }

        private async Task LoadScenesFromBundleAsync(SceneBundle sceneBundle)
        {
            await sceneBundle.LoadScenesAsync(
                onSceneProgress: (sceneName, progress) =>
                {
                    GameLogger.Log($"{sceneName}: {progress * 100f}%", LogCategory.Unity);
                },
                onTotalProgress: (progress) =>
                {
                    GameLogger.Log($"Total Progress - {progress * 100f}%", LogCategory.Unity);
                });
        }
    }
}
