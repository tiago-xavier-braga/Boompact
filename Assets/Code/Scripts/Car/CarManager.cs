using System.Collections.Generic;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviEssencials.Shared;

namespace XaviGames.Car
{
    public class CarManager : MonoBehaviour
    {
        [field: Header("Car Parameters")]
        [field: SerializeField]
        public float TopSpeed { get; private set; }

        [field: SerializeField]
        public float Acceleration { get; private set; }

        [field: SerializeField]
        public float BreakForce { get; private set; }

        [field: SerializeField]
        public float SteeringRange { get; private set; }

        [field: SerializeField]
        public float SteeringRangeAtMaxSpeed { get; private set; }

        [field: SerializeField]
        public float CentreOfGravityOffset { get; private set; }

        [field: Header("Scripts References")]
        [field: SerializeField]
        public BoolEventChannel CarMovementPermission { get; private set; }

        [field: SerializeField]
        public CarMovementController CarController { get; private set; }

        [field: SerializeField]
        public List<WheelController> WheelControllers { get; private set; }

        [Button("Switch Car Movement Permission", true)]
        public void SwitchCarMovementPermission()
        {
            CarMovementPermission.RaiseEvent(!CarMovementPermission.Value);
        }
    }
}