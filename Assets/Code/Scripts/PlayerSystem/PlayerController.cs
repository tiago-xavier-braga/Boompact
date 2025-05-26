using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using XaviEssencials.Runtime;
using XaviGames.Cameras;
using XaviGames.Car;
using XaviGames.Server;
using XaviGames.Services;

namespace XaviGames.PlayerSystem
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField]
        private CarDatabase _carDatabase;

        [field: SerializeField]
        public UserSession UserSession { get; private set; }

        [SerializeField]
        private PlayerInput _playerInput;

        [SerializeField]
        private GameObject _carVirtualCamera;

        [SerializeField]
        [ReadOnly]
        private GameObject _spawnedCar;

        [SerializeField]
        [ReadOnly]
        private GameObject _spawnedVirtualCamera;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                _playerInput.enabled = false;
                return;
            }

            RegisterCarIdServer();
        }

        public ClientCarReference RegisterCarIdServer()
        {
            GameLogger.Log($"Player {OwnerClientId} registered car ID: {UserSession.CarParameter.Id}", LogCategory.Client);
            
            return new ClientCarReference
            {
                ClientId = OwnerClientId,
                CarId = UserSession.CarParameter.Id
            };
        }

        [ClientRpc]
        private void NotifyClientCarSpawnedClientRpc(ulong carNetId)
        {
            if (!IsOwner)
            {
                return;
            }

            NetworkObject carNetworkObject = NetworkManager.SpawnManager.SpawnedObjects[carNetId];
            if (carNetworkObject == null)
            {
                GameLogger.LogError("Car network object not found on client", LogCategory.Client);
                return;
            }

            _spawnedCar = carNetworkObject.gameObject;

            _spawnedVirtualCamera = Instantiate(_carVirtualCamera);
            if (_spawnedVirtualCamera.TryGetComponent(out CarFollowCamera virtualCam))
            {
                virtualCam.SetFollowTransform(_spawnedCar.transform);
            }
            else
            {
                GameLogger.LogError("VirtualCamera component not found on prefab", LogCategory.Client);
            }
        }
    }
}
