using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XaviEssencials;

namespace XaviGames.ArcadeCar
{
    [RequireComponent(typeof(ArcadeCarManager))]
    public class ArcadeCarController : MonoBehaviour
    {
        [Header("Car Properties")]
        [SerializeField]
        private List<ArcadeWheelReference> _wheelsColliders;

        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _inputVector;

        private ArcadeCarManager _arcadeCarManager;
        private Rigidbody rigidBody;

        private void Start()
        {
            _arcadeCarManager = GetComponent<ArcadeCarManager>();
            rigidBody = GetComponent<Rigidbody>();

            Vector3 centerOfMass = rigidBody.centerOfMass;
            centerOfMass.y += _arcadeCarManager.CentreOfGravityOffset;
            rigidBody.centerOfMass = centerOfMass;
        }

        private void FixedUpdate()
        {
            float hInput = _inputVector.x;
            float vInput = _inputVector.y;

            float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.linearVelocity);
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
        }

        public void OnMoveInput(InputAction.CallbackContext context)
        {
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
    }
}

