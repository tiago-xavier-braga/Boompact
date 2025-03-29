using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using XaviEssencials;

namespace XaviGames.Network
{
    public class NetworkSceneController : MonoBehaviour
    {
        [Serializable]
        public enum BuildMode
        {
            Server,
            Client
        }


        [SerializeField]
        private BuildMode _buildMode;

        [SerializeField]
        private SceneReference _clientScene; 

        [SerializeField]
        private SceneReference _serverScene;

        private void Start()
        {
            if (_buildMode == BuildMode.Server)
            {
                NetworkManager.Singleton.StartServer();
                    SceneManager.LoadScene(_serverScene.SceneName);
            }
            else
            {
                NetworkManager.Singleton.StartClient();
                SceneManager.LoadScene(_clientScene.SceneName);

            }
        }
    }
}
