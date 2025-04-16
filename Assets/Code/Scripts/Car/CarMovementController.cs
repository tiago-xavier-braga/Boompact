using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using XaviEssencials.Runtime;

namespace XaviGames.Car
{
    public class CarMovementController : NetworkBehaviour
    {
        [Header("Car Properties")]
        [SerializeField]
        private PlayerInput _playerInput;

        [SerializeField]
        private CarManager _carManager;

        [SerializeField]
        private Rigidbody _rigidBody;

        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _inputVector;

        [SerializeField]
        [ReadOnly]
        private float _kmPerHour = 0f;

        private CarParameter _carParameter;
        private bool _canMove;

        public override void OnNetworkSpawn()
        {
            _carParameter = _carManager.CarParameter;

            Vector3 centerOfMass = _rigidBody.centerOfMass;
            centerOfMass.y += _carParameter.CentreOfGravityOffset;
            _rigidBody.centerOfMass = centerOfMass;

            _playerInput.enabled = IsOwner;
            base.OnNetworkSpawn();
        }

        private void FixedUpdate()
        {
            _canMove = _carManager.CarMovementPermission.Value;
            if (!_canMove)
            {
                return;
            }

            _kmPerHour = _rigidBody.linearVelocity.magnitude * 3.6f;

            float forwardSpeed = Vector3.Dot(transform.forward, _rigidBody.linearVelocity);
            float speedFactor = Mathf.InverseLerp(0, _carParameter.TopSpeed, Mathf.Abs(forwardSpeed));

            float currentMotorTorque = Mathf.Lerp(_carParameter.Acceleration, 0, speedFactor);
            float currentSteerRange = Mathf.Lerp(
                _carParameter.SteeringRange, _carParameter.SteeringRangeAtMaxSpeed, speedFactor);

            bool isAccelerating = Mathf.Sign(_inputVector.y) == Mathf.Sign(forwardSpeed);

            ApplyWheelForces(currentMotorTorque, currentSteerRange, isAccelerating);
        }

        public void OnMoveInput(InputAction.CallbackContext context)
        {
            if (!IsOwner || !_canMove)
            {
                return;
            }

            _inputVector = context.ReadValue<Vector2>();
        }

        private void ApplyWheelForces(float currentMotorTorque, float currentSteerRange, bool isAccelerating)
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
                    wheel.WheelCollider.brakeTorque = Mathf.Abs(_inputVector.y) * _carParameter.BreakForce;
                }

                wheel.UpdateWheelPosition();
            }
        }
    }
}
