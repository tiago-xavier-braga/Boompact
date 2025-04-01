using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace XaviGames.Car
{
    public class CarManager : NetworkBehaviour
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
        public CarMovementController CarController { get; private set; }

        [field: SerializeField]
        public List<WheelController> WheelControllers { get; private set; }

        [field: Header("Network References")]
        [field: SerializeField]
        public CarNetworkSync CarNetworkSync { get; private set; }
    }
}