using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Car;
using XaviGames.PlayerSystem;

namespace XaviGames.Services
{
    public class UserSession : MonoBehaviour
    {
        [field: SerializeField]
        public CarParameter CarParameter { get; private set; } = null;

        public static UserSession Instance { get; private set; } = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SerCarPrefab(CarParameter parameter)
        {
            if (parameter is null)
            {
                GameLogger.LogError("CarPrefab is null", LogCategory.Unity);
                return;
            }

            CarParameter = parameter;
            return;
        }
    }
}
