using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Car;

namespace XaviGames.Services
{
    [CreateAssetMenu(fileName = "UserSession", menuName = "Xavi Games/Services/User Sessions")]
    public class UserSession : ScriptableObject
    {
        [field: SerializeField]
        public CarParameter CarParameter { get; private set; } = null;

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
