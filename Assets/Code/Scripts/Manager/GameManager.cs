using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Manager
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private int _fps = 60;

        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _fps;
        }
    }
}
