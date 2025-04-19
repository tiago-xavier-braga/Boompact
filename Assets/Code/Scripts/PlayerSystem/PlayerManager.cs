using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Cameras;
using XaviGames.Car;
using XaviGames.Services;

namespace XaviGames.PlayerSystem
{
    public class PlayerManager : NetworkBehaviour
    {
        [SerializeField]
        private CarDatabase _carDatabase;

        [SerializeField]
        private UserSession _userSession;

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
                return;
            }

            CarParameter carParameter = _userSession.CarParameter;
            SpawnCarServerRpc(carParameter.Id);
        }

        [ServerRpc(RequireOwnership = true)]
        private void SpawnCarServerRpc(string id)
        {
            CarParameter parameter = _carDatabase.GetCarParameter(id);

            if (parameter == null || parameter.CarGameObject == null)
            {
                GameLogger.LogError($"CarParameter not found or GameObject is null for ID: {id}", LogCategory.Server);
                return;
            }

            GameObject car = Instantiate(parameter.CarGameObject, transform.position, Quaternion.identity);
            var networkObject = car.GetComponent<NetworkObject>();
            networkObject.SpawnWithOwnership(OwnerClientId);

            NotifyClientCarSpawnedClientRpc(networkObject.NetworkObjectId);
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
            if (_spawnedVirtualCamera.TryGetComponent(out VirtualCamera virtualCam))
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
