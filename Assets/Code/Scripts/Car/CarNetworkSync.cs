using Unity.Netcode;
using UnityEngine;

namespace XaviGames.Car
{
    public class CarNetworkSync : NetworkBehaviour
    {
        [SerializeField]
        private CarManager _carManager;

        [ServerRpc]
        public void SendWheelForcesToServerRpc(float motorTorque, float steerRange, bool isAccelerating)
        {
            UpdateWheelForcesToServerClientRpc(motorTorque, steerRange, isAccelerating);
        }

        [ClientRpc]
        private void UpdateWheelForcesToServerClientRpc(float motorTorque, float steerRange, bool isAccelerating)
        {
            if (!IsOwner)
            {
                _carManager.CarController.ApplyWheelForces(motorTorque, steerRange, isAccelerating);
            }
        }
    }
}

