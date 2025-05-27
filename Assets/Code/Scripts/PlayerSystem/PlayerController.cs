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
    }
}
