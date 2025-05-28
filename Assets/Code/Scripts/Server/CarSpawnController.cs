//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Car;
using XaviGames.PlayerSystem;

namespace XaviGames.Server
{
    public class CarSpawnController : MonoBehaviour
    {
        [SerializeField]
        private CarDatabase _carDatabase;

        [SerializeField]
        private List<Transform> _spawnPoints;

        private List<ClientCarReference> _playersCars = new();

        public void SpawnAllCars()
        {
            var clients = NetworkManager.Singleton.ConnectedClientsList;
            GameLogger.Log($"Spawning cars for {clients.Count} players", LogCategory.Server);

            for (int i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                var playerObj = client.PlayerObject;
                var playerController = playerObj.GetComponent<PlayerController>();
                var idRef = playerController.RegisterCarIdServer();

                var param = _carDatabase.GetCarParameter(idRef.CarId);
                if (param == null)
                {
                    continue;
                }

                var spawn = _spawnPoints[i];
                var car = Instantiate(param.CarGameObject, spawn.position, spawn.rotation);
                //var car = Instantiate(param.CarGameObject, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));

                var netObj = car.GetComponent<NetworkObject>();
                netObj.SpawnWithOwnership(idRef.ClientId);
            }
        }
    }
}