using System;
using UnityEngine;

namespace XaviGames.ArcadeCar
{
    [Serializable]
    public class ArcadeWheelReference
    {
        [field: SerializeField]
        public Transform ModelTransform { get; private set; }

        [field: SerializeField]
        public WheelCollider WheelCollider {  get; private set; }

        [field: SerializeField]
        public bool IsMotorized { get; private set; }

        [field: SerializeField]
        public bool IsSteerable { get; private set; }
    }
}
