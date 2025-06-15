//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using XaviGames.Cameras;

namespace XaviGames.Car
{
    public class CarCameraSetup : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _virtualCameraPrefab;

        private GameObject _virtualCamera;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                return;
            }

            _virtualCamera = Instantiate(_virtualCameraPrefab);
            _virtualCamera.GetComponent<CarFollowCamera>().SetFollowTransform(transform);
        }

        public override void OnNetworkDespawn()
        {
            if (_virtualCamera != null)
            {
                Destroy(_virtualCamera);
            }
        }
    }
}
