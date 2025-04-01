using UnityEngine;
using UnityEngine.SceneManagement;
using XaviEssencials;

namespace XavieGames.Manager
{
    public class StartupManager : MonoBehaviour
    {
        public SceneReference _clientScene;

        public SceneReference _serverScene;

        [Header("Build Settings")]
        [SerializeField]
        private bool _isClientBuild;

        private void Start()
        {
            if (_isClientBuild)
            {
                SceneManager.LoadScene(_clientScene.SceneName);
            }
            else
            {
                SceneManager.LoadScene(_serverScene.SceneName);
            }
        }
    }
}
