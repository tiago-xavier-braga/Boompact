using System;
using Unity.Netcode;
using UnityEngine;

namespace XaviGames.Car
{
    public class WheelController : NetworkBehaviour
    {
        [field: SerializeField]
        public Transform ModelTransform { get; private set; }

        [field: SerializeField]
        public WheelCollider WheelCollider {  get; private set; }

        [field: SerializeField]
        public bool IsMotorized { get; private set; }

        [field: SerializeField]
        public bool IsSteerable { get; private set; }

        public void UpdateWheelPosition()
        {
            if (!IsOwner)
            {
                return;
            }

            Vector3 position;
            Quaternion rotation;
            WheelCollider.GetWorldPose(out position, out rotation);
            ModelTransform.position = position;
            ModelTransform.rotation = rotation;
        }
    }
}
