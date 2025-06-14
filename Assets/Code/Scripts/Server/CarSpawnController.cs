//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Car;
using XaviGames.PlayerSystem;

namespace XaviGames.Host
{
    public class CarSpawnController : MonoBehaviour
    {
        [SerializeField]
        private CarDatabase _carDatabase;

        [SerializeField]
        private List<Transform> _spawnPoints;

        public void SpawnAllCars()
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                GameLogger.LogError("CarSpawnController can only be used on the server.", LogCategory.Server);
                return;
            }

            var clients = NetworkManager.Singleton.ConnectedClientsList;
            GameLogger.Log($"Spawning cars for {clients.Count} players", LogCategory.Server);

            for (int i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                var playerObj = client.PlayerObject;
                var playerController = playerObj.GetComponent<PlayerController>();

                string carId = playerController.GetCarId();
                if (string.IsNullOrEmpty(carId))
                {
                    GameLogger.LogWarning($"Player {client.ClientId} has no car ID set. Skipping car spawn.", LogCategory.Server);
                    continue;
                }

                var param = _carDatabase.GetCarParameter(carId);
                if (param == null)
                {
                    continue;
                }

                var spawn = _spawnPoints[i];
                var car = Instantiate(param.CarGameObject, spawn.position, spawn.rotation);
                car.name = $"Car_{client.ClientId}_{carId}";
                playerController.SetCarGameObject(car);
                var netObj = car.GetComponent<NetworkObject>();
                netObj.SpawnWithOwnership(client.ClientId);
            }
        }
    }
}