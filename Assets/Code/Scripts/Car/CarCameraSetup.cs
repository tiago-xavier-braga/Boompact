//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using Unity.Cinemachine;
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

            CinemachineCamera virtualCameraComponent = _virtualCamera.GetComponent<CinemachineCamera>();
            CameraTarget cameraTarget = virtualCameraComponent.Target;
            cameraTarget.TrackingTarget = transform;
            virtualCameraComponent.Target = cameraTarget;
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
