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
        private List<Transform> spawnPoints;

        private List<ClientCarReference> _playersCars = new();

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        private void OnDisable()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }

        private void OnClientConnected(ulong clientId)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
            {
                var playerObject = networkClient.PlayerObject;
                if (playerObject != null && playerObject.TryGetComponent<PlayerController>(out var playerController))
                {
                    _playersCars.Add(playerController.RegisterCarIdServer());
                }
                else
                {
                    Debug.LogWarning($"PlayerController not found on player object of client {clientId}");
                }
            }
            else
            {
                Debug.LogWarning($"Client {clientId} not found in ConnectedClients.");
            }
        }

        //public void SpawnAllCars()
        //{

        //    GameLogger.Log($"Spawning cars for {_playersCars.Count} players", LogCategory.Server);
        //    int indexPlayer = 0;
        //    foreach (var reference in _playersCars)
        //    {
        //        ulong clientId = reference.ClientId;
        //        string carId = reference.CarId;

        //        CarParameter parameter = _carDatabase.GetCarParameter(carId);
        //        if (parameter == null || parameter.CarGameObject == null)
        //        {
        //            GameLogger.LogError($"Invalid car config for client {clientId} with ID {carId}", LogCategory.Server);
        //            continue;
        //        }

        //        Transform spawnPoint = spawnPoints[indexPlayer];
        //        indexPlayer++;

        //        GameObject car = Instantiate(parameter.CarGameObject, Vector3.zero, Quaternion.Euler(0f,0f,0f));

        //        var netObj = car.GetComponent<NetworkObject>();
        //        netObj.SpawnWithOwnership(clientId);
        //    }
        //}

        public void SpawnAllCars()
        {
            var clients = NetworkManager.Singleton.ConnectedClientsList;
            GameLogger.Log($"Spawning cars for {clients.Count} players", LogCategory.Server);

            for (int i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                var playerObj = client.PlayerObject;
                var pc = playerObj.GetComponent<PlayerController>();
                var idRef = pc.RegisterCarIdServer();

                var param = _carDatabase.GetCarParameter(idRef.CarId);
                if (param == null)
                {
                    continue;
                }

                var spawn = spawnPoints[i];
                //var car = Instantiate(param.CarGameObject, spawn.position, spawn.rotation);
                var car = Instantiate(param.CarGameObject, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));

                var netObj = car.GetComponent<NetworkObject>();
                netObj.SpawnWithOwnership(idRef.ClientId);
            }
        }
    }
}