using System.Collections.Generic;
using System.Linq;
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
        private CarParameter _carParameter;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private List<WheelController> _wheelControllers;

        [Header("Input References")]
        [SerializeField]
        private InputActionReference _moveInputAction;

        [SerializeField]
        private InputActionReference _handbrakeInputAction;

        [Header("Car Evetns")]
        [SerializeField]
        private EventChannel _onCarDrifting;

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

        private WheelFrictionCurve _originalSidewaysFriction = new();
        private WheelFrictionCurve _driftSidewaysFriction = new();
        
        private float _defaultAngularDamping;

        public void OnEnable()
        {
            _moveInputAction.action.performed += OnMoveInput;
            _moveInputAction.action.canceled += OnMoveInput;
            _handbrakeInputAction.action.performed += OnHandbrake;
            _handbrakeInputAction.action.canceled += OnHandbrake;
        }

        public void OnDisable()
        {
            _moveInputAction.action.performed -= OnMoveInput;
            _moveInputAction.action.canceled -= OnMoveInput;
            _handbrakeInputAction.action.performed -= OnHandbrake;
            _handbrakeInputAction.action.canceled -= OnHandbrake;
        }

        public override void OnNetworkSpawn()
        {
            ApplyCenterMass();
            ConfigureWheelSettings();

            base.OnNetworkSpawn();
        }

        private void FixedUpdate()
        {
            UpdatePhysics();
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
                    _onCarDrifting?.Raise(true);
                    break;

                case InputActionPhase.Canceled:
                    ApplyDefaultWheelSettings();
                    _onCarDrifting?.Raise(false);
                    break;
            }
        }
        private void ApplyCenterMass()
        {
            Vector3 centerOfMass = _rigidbody.centerOfMass;
            centerOfMass.y += _carParameter.CentreOfGravityOffset;
            _rigidbody.centerOfMass = centerOfMass;
        }

        private void ConfigureWheelSettings()
        {
            _originalSidewaysFriction = _wheelControllers.First().WheelCollider.sidewaysFriction;
            _driftSidewaysFriction = _carParameter.DriftFrictionCurve;
            _defaultAngularDamping = _rigidbody.angularDamping;
        }


        private void ApplyDriftWheelSettings()
        {
            foreach (var wheel in _wheelControllers)
            {
                wheel.WheelCollider.sidewaysFriction = _driftSidewaysFriction;
            }

            _rigidbody.angularDamping = _carParameter.DriftAngularDamping;
        }

        private void ApplyDefaultWheelSettings()
        {
            foreach (var wheel in _wheelControllers)
            {
                wheel.WheelCollider.sidewaysFriction = _originalSidewaysFriction;
            }

            _rigidbody.angularDamping = _defaultAngularDamping;
        }

        private void UpdatePhysics()
        {
            _kmPerHour = _rigidbody.linearVelocity.magnitude * 3.6f;

            float forwardSpeed = Vector3.Dot(transform.forward, _rigidbody.linearVelocity);
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
