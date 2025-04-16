using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Car
{
    [CreateAssetMenu(fileName = "CarDatabase", menuName = "Xavi Games/Car/Car Database")]
    public class CarDatabase : ScriptableObject
    {
        [field: SerializeField]
        public List<CarParameter> CarsParameters { get; private set; } = new List<CarParameter>();

        public CarParameter GetCarParameter(string id)
        {
            var carParameter = CarsParameters.FirstOrDefault(parameter => parameter.Id == id);

            if (carParameter is null)
            {
                GameLogger.LogWarning(
                    $"Car with id {id} not found in the database. Returning default car.", LogCategory.Unity);
            }

            return carParameter;
        }

    }
}
