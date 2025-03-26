using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using XaviEssencials;

namespace XaviGames.ArcadeCar
{
    [RequireComponent(typeof(ArcadeCarManager))]
    public class ArcadeCarController : NetworkBehaviour
    {
        [Header("Car Properties")]
        [SerializeField]
        private List<ArcadeWheelReference> _wheelsColliders;

        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _inputVector;

        private ArcadeCarManager _arcadeCarManager;
        private Rigidbody _rigidBody;

        public NetworkVariable<Vector3> CarPosition = new NetworkVariable<Vector3>(
            Vector3.zero,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );

        public override void OnNetworkSpawn()
        {
            _arcadeCarManager = GetComponent<ArcadeCarManager>();
            _rigidBody = GetComponent<Rigidbody>();

            Vector3 centerOfMass = _rigidBody.centerOfMass;
            centerOfMass.y += _arcadeCarManager.CentreOfGravityOffset;
            _rigidBody.centerOfMass = centerOfMass;

            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            _arcadeCarManager.PlayerInput.enabled = false;
            base.OnNetworkDespawn();
        }

        private void FixedUpdate()
        {
            float hInput = _inputVector.x;
            float vInput = _inputVector.y;

            float forwardSpeed = Vector3.Dot(transform.forward, _rigidBody.linearVelocity);
            float speedFactor = Mathf.InverseLerp(0, _arcadeCarManager.TopSpeed, Mathf.Abs(forwardSpeed));

            float currentMotorTorque = Mathf.Lerp(_arcadeCarManager.Acceleration, 0, speedFactor);
            float currentSteerRange = Mathf.Lerp(
                _arcadeCarManager.SteeringRange, _arcadeCarManager.SteeringRangeAtMaxSpeed, speedFactor);

            bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

            foreach (var wheel in _wheelsColliders)
            {
                if (wheel.IsSteerable)
                {
                    wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
                }

                if (isAccelerating)
                {
                    if (wheel.IsMotorized)
                    {
                        wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
                    }

                    wheel.WheelCollider.brakeTorque = 0f;
                }
                else
                {
                    wheel.WheelCollider.motorTorque = 0f;
                    wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * _arcadeCarManager.BreakForce;
                }

                UpdateWheelPosition(wheel);
            }

            if (IsOwner)
            {
                SendPositionToServerRpc(transform.position);
            }
        }

        public void OnMoveInput(InputAction.CallbackContext context)
        {
            if (!IsOwner)
            {
                return;
            }

            _inputVector = context.ReadValue<Vector2>();
        }

        private void UpdateWheelPosition(ArcadeWheelReference wheel)
        {
            Vector3 position;
            Quaternion rotation;
            wheel.WheelCollider.GetWorldPose(out position, out rotation);
            wheel.ModelTransform.position = position;
            wheel.ModelTransform.rotation = rotation;
        }

        [ServerRpc]
        private void SendPositionToServerRpc(Vector3 position)
        {
            UpdatePositionForClientsClientRpc(position);
        }

        [ClientRpc]
        private void UpdatePositionForClientsClientRpc(Vector3 position)
        {
            if (!IsOwner)
            {
                transform.position = position;
            }
        }
    }
}
