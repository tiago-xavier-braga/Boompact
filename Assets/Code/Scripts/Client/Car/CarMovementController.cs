using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using XaviEssencials;

namespace XaviGames.Car
{
    public class CarMovementController : NetworkBehaviour
    {
        [Header("Car Properties")]
        [SerializeField]
        private CarManager _carManager;

        [SerializeField]
        private Rigidbody _rigidBody;

        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _inputVector;

        public override void OnNetworkSpawn()
        {
            Vector3 centerOfMass = _rigidBody.centerOfMass;
            centerOfMass.y += _carManager.CentreOfGravityOffset;
            _rigidBody.centerOfMass = centerOfMass;

            base.OnNetworkSpawn();
        }

        private void FixedUpdate()
        {
            float forwardSpeed = Vector3.Dot(transform.forward, _rigidBody.linearVelocity);
            float speedFactor = Mathf.InverseLerp(0, _carManager.TopSpeed, Mathf.Abs(forwardSpeed));

            float currentMotorTorque = Mathf.Lerp(_carManager.Acceleration, 0, speedFactor);
            float currentSteerRange = Mathf.Lerp(
                _carManager.SteeringRange, _carManager.SteeringRangeAtMaxSpeed, speedFactor);

            bool isAccelerating = Mathf.Sign(_inputVector.y) == Mathf.Sign(forwardSpeed);

            ApplyWheelForces(currentMotorTorque, currentSteerRange, isAccelerating);

            if (IsOwner)
            {
                _carManager.CarNetworkSync.SendWheelForcesToServerRpc(
                    currentMotorTorque, currentSteerRange, isAccelerating);
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

        public void ApplyWheelForces(float currentMotorTorque, float currentSteerRange, bool isAccelerating)
        {
            foreach (var wheel in _carManager.WheelControllers)
            {
                if (wheel.IsSteerable)
                {
                    wheel.WheelCollider.steerAngle = _inputVector.x * currentSteerRange;
                }

                if (isAccelerating)
                {
                    if (wheel.IsMotorized)
                    {
                        wheel.WheelCollider.motorTorque = _inputVector.y * currentMotorTorque;
                    }

                    wheel.WheelCollider.brakeTorque = 0f;
                }
                else
                {
                    wheel.WheelCollider.motorTorque = 0f;
                    wheel.WheelCollider.brakeTorque = Mathf.Abs(_inputVector.y) * _carManager.BreakForce;
                }

                wheel.UpdateWheelPosition();
            }
        }
    }
}
