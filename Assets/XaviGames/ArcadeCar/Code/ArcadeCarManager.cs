using UnityEngine;

namespace XaviGames.ArcadeCar
{
    public class ArcadeCarManager : MonoBehaviour
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
    }
}