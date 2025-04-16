using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
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
        [ReadOnly]
        private GameObject _spawnedCar;

        public override void OnNetworkSpawn()
        {
            CarParameter carParameter = _userSession.CarParameter;

            if (IsOwner)
            {
                SpawnCarServerRpc(carParameter.Id, OwnerClientId);
            }

            base.OnNetworkSpawn();
        }

        [ServerRpc(RequireOwnership = true)]
        public void SpawnCarServerRpc(string id, ulong clientId)
        {
            CarParameter parameter = _carDatabase.GetCarParameter(id);

            if (parameter == null || parameter.CarGameObject == null)
            {
                GameLogger.LogError($"CarParameter not found or GameObject is null for ID: {id}", LogCategory.Server);
                return;
            }

            _spawnedCar = Instantiate(parameter.CarGameObject, transform.position, Quaternion.identity);
            _spawnedCar.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        }
    }
}