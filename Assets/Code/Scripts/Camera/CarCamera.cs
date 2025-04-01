using Unity.Netcode;
using UnityEngine;

namespace XaviGames.Cameras
{
    public class CarCamera : NetworkBehaviour
    {
        [SerializeField]
        private Camera _camera;

        private void Start()
        {
            if (!IsOwner)
            {
                _camera.gameObject.SetActive(false);
            }
        }
    }
}