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

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                return;
            }

            var cam = Instantiate(_virtualCameraPrefab);
            cam.GetComponent<CarFollowCamera>().SetFollowTransform(transform);
        }
    }
}
