using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using XaviEssencials.Runtime;

namespace XaviGames.Car
{
    public class CarMovementController : NetworkBehaviour
    {
        [Header("Car Properties")]
        [SerializeField]
        private CarParameter _carParameter;
        
        [SerializeField]
        private PlayerInput _playerInput;

        [SerializeField]
        private Rigidbody _rigidBody;

        [SerializeField]
        private List<WheelController> _wheelControllers;

        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _inputVector;

        [SerializeField]
        [ReadOnly]
        private bool _inputHandbrake;

        [SerializeField]
        [ReadOnly]
        private float _kmPerHour = 0f;

        private WheelFrictionCurve _defaultSidewaysFriction = new();
        private WheelFrictionCurve _driftSidewaysFriction = new();
        
        private float _defaultAngularDamping;

        [SerializeField]
        private float _driftAngularDamping;

        public override void OnNetworkSpawn()
        {
            _playerInput.enabled = IsOwner;
            ApplyCenterMass();
            ConfigureWheelSettings();

            base.OnNetworkSpawn();
        }

        private void FixedUpdate()
        {
            UpdateCarPhysics();
        }

        public void OnMoveInput(InputAction.CallbackContext context)
        {
            if (!IsOwner)
            {
                return;
            }

            _inputVector = context.ReadValue<Vector2>();
        }

        public void OnHandbrake(InputAction.CallbackContext context)
        {
            if (!IsOwner)
            {
                return;
            }

            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    ApplyDriftWheelSettings();
                    break;

                case InputActionPhase.Canceled:
                    ApplyDefaultWheelSettings();
                    break;
            }
        }
        private void ApplyCenterMass()
        {
            Vector3 centerOfMass = _rigidBody.centerOfMass;
            centerOfMass.y += _carParameter.CentreOfGravityOffset;
            _rigidBody.centerOfMass = centerOfMass;
        }

        private void ConfigureWheelSettings()
        {
            _defaultSidewaysFriction = _wheelControllers.First().WheelCollider.sidewaysFriction;
            _driftSidewaysFriction = _carParameter.DriftFrictionCurve;
            _defaultAngularDamping = _rigidBody.angularDamping;
            _driftAngularDamping = 0.3f;
        }


        private void ApplyDriftWheelSettings()
        {
            foreach (var wheel in _wheelControllers)
            {
                wheel.WheelCollider.sidewaysFriction = _driftSidewaysFriction;
            }

            _rigidBody.angularDamping = _driftAngularDamping;
        }

        private void ApplyDefaultWheelSettings()
        {
            foreach (var wheel in _wheelControllers)
            {
                wheel.WheelCollider.sidewaysFriction = _defaultSidewaysFriction;
            }

            _rigidBody.angularDamping = _defaultAngularDamping;
        }

        private void UpdateCarPhysics()
        {
            _kmPerHour = _rigidBody.linearVelocity.magnitude * 3.6f;

            float forwardSpeed = Vector3.Dot(transform.forward, _rigidBody.linearVelocity);
            float speedFactor = Mathf.InverseLerp(0f, _carParameter.TopSpeed, Mathf.Abs(forwardSpeed));

            float motorTorque = Mathf.Lerp(_carParameter.Acceleration, 0f, speedFactor);
            float steeringRange = Mathf.Lerp(
                _carParameter.SteeringRange,
                _carParameter.SteeringRangeAtMaxSpeed,
                speedFactor
            );

            bool isAccelerating = Mathf.Sign(_inputVector.y) == Mathf.Sign(forwardSpeed);

            ApplyWheelForces(motorTorque, steeringRange, isAccelerating);
        }

        private void ApplyWheelForces(float currentMotorTorque, float currentSteerRange, bool isAccelerating)
        {
            foreach (var wheel in _wheelControllers)
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
