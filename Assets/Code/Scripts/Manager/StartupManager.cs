using System.Threading.Tasks;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Services;

namespace XaviGames.Manager
{
    public class StartupManager : MonoBehaviour
    {
        [SerializeField]
        private SceneBundle _clientSceneBundle;


        [SerializeField]
        private SceneBundle _serverSceneBundle;

        [Header("Build Settings")]
        [SerializeField]
        private ServicesSettings _servicesSettings;

        private async void Start()
        {
#if UNITY_SERVER
            await LoadScenesFromBundleAsync(_serverSceneBundle);
#else
            await LoadScenesFromBundleAsync(_clientSceneBundle);
#endif

#if UNITY_EDITOR
            var buildType = _servicesSettings.BuildType;
            if (buildType == BuildType.Client)
            {
                await LoadScenesFromBundleAsync(_clientSceneBundle);
            }
            else
            {
                await LoadScenesFromBundleAsync(_serverSceneBundle);
            }
#endif
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
