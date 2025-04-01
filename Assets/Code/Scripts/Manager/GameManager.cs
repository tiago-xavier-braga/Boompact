using Unity.Netcode;
using UnityEngine;
using XaviEssencials;

namespace XaviGames.Manager
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private int _fps = 60;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _fps;
        }
    }
}
