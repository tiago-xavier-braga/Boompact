using System.Collections.Generic;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviEssencials.Shared;

namespace XaviGames.Car
{
    public class CarManager : MonoBehaviour
    {
        [field: Header("Scripts References")]
        [field: SerializeField]
        public CarParameter CarParameter { get; private set; }

        [field: SerializeField]
        public BoolEventChannel CarMovementPermission { get; private set; }

        [field: SerializeField]
        public CarMovementController CarController { get; private set; }

        [field: SerializeField]
        public List<WheelController> WheelControllers { get; private set; }

        [Button("Switch Car Movement Permission", true )]
        public void SwitchCarMovementPermission()
        {
            CarMovementPermission.RaiseEvent(!CarMovementPermission.Value);
        }
    }
}