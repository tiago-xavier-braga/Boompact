using System;
using Unity.Netcode;
using Unity.Services.Core;
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
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _fps;

            InitMultiplayerSDK();
        }

        private async void InitMultiplayerSDK()
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}
