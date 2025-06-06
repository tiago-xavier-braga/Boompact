using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using XaviEssencials.Runtime;

namespace XaviGames.Manager
{
    public class NetworkSceneLoader : MonoBehaviour
    {
        private TaskCompletionSource<bool> _sceneLoadTcs;

        public static NetworkSceneLoader Instance { get; private set; }

        public async Task LoadSceneAsyncServer(SceneReference scene)
        {
            if (!NetworkManager.Singleton.IsServer)
            { 
                    return;
            }

            if (scene is null)
            {
                Debug.LogError("SceneReference is null. Cannot load scene.");
                return;
            }

            _sceneLoadTcs = new TaskCompletionSource<bool>();
            
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            
            NetworkManager.Singleton.SceneManager.LoadScene(scene.SceneName, LoadSceneMode.Single);
            
            await _sceneLoadTcs.Task;
        }

        private void OnSceneEvent(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
            {
                GameLogger.Log($"Scene {sceneEvent.SceneName} loaded successfully.", LogCategory.Client);
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
                _sceneLoadTcs.TrySetResult(true);
            }
        }
    }
}
