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

        private void Start()
        {
            _ = LoadScenesFromBundleAsync(_clientSceneBundle);
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
