// Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using XaviEssencials.Runtime;
using XaviGames.Car;
using XaviGames.Services;

namespace XaviGames.PlayerSystem
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField]
        private CarDatabase _carDatabase;

        [field: SerializeField]
        public UserSession UserSession { get; private set; }

        [field: SerializeField]
        [field: ReadOnly]
        public GameObject CarSpawned { get; private set; } = null;

        [SerializeField]
        private PlayerInput _playerInput;

        private string _carId = string.Empty;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                _playerInput.enabled = false;
                return;
            }

            string id = UserSession.CarParameter.Id;
            SubmitCarIdServerRpc(id);
        }

        [ServerRpc(RequireOwnership = true)]
        private void SubmitCarIdServerRpc(string carId)
        {
            _carId = carId;
        }

        public string GetCarId()
        {
            return _carId;
        }

        public void SetCarGameObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                GameLogger.LogError("CarSpawned is null. Cannot set car game object.", LogCategory.Unity);
                return;
            }

            CarSpawned = gameObject;
            GameLogger.Log($"Car game object set for player {OwnerClientId}. Car Name: {CarSpawned.name}", LogCategory.Unity);
        }
    }
}
