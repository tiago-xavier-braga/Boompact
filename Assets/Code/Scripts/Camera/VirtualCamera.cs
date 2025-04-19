using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using Unity.Netcode;
using XaviEssencials.Runtime;

namespace XaviGames.Cameras
{
    public class VirtualCamera : MonoBehaviour
    {
        [SerializeField]
        private CameraOption cameraOption;

        [field: Header("Follow Target Settings")]
        [field: SerializeField]
        public Transform FollowTransform { get; private set; }

        [SerializeField]
        private float _orbitRadius;

        [SerializeField]
        private float _heightOffset = 3f;

        [SerializeField]
        private float _rotationSpeed = 120f;

        [SerializeField]
        private float _smoothTime = 0.1f;

        [field: Header("Follow Spline Settings")]
        [field: SerializeField]
        public SplineContainer SplineContainer { get; private set; }

        [Range(0f, 1f)]
        [SerializeField]
        private float _normalizedPosition;

        [SerializeField]
        private float _speed = 0.2f;

        [SerializeField]
        private bool _loop = false;

        [SerializeField]
        private bool _lookForward = true;

        [SerializeField]
        private Vector3 _offset;

        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _cameraInput;

        private Vector3 _currentVelocity;
        private float _currentYaw;
        private float _splineLength;

        private void Start()
        {
            if (SplineContainer != null)
            {
                UpdateSplineLength();
            }
        }

        private void Update()
        {
            switch (cameraOption)
            {
                case CameraOption.Fixed:
                    {
                        break;
                    }
                case CameraOption.FollowSpline:
                    {
                        ExecuteFollowSpline();
                        break;
                    }
                case CameraOption.FollowTarget:
                    {
                        ExecuteFollowTarget();
                        break;
                    }
                default:
                    {
                        Debug.LogWarning($"Unhandled CameraOption: {cameraOption}");
                        break;
                    }
            }
        }

        public void OnCameraInput(InputAction.CallbackContext context)
        {
            _cameraInput = context.ReadValue<Vector2>();
        }

        public void SetFollowTransform(Transform transform)
        {
            if (transform == null)
            {
                GameLogger.LogError("Transform is null", LogCategory.Unity);
                return;
            }

            FollowTransform = transform;
        }

        public void SetSplineContainer(SplineContainer splineContainer)
        {
            if (splineContainer == null)
            {
                GameLogger.LogError("SplineContainer is null", LogCategory.Unity);
                return;
            }

            SplineContainer = splineContainer;
            UpdateSplineLength();
        }

        private void UpdateSplineLength()
        {
            _splineLength = SplineContainer.Spline.GetLength();
        }

        private void ExecuteFollowSpline()
        {
            _normalizedPosition += _speed * Time.deltaTime / _splineLength;

            if (_loop)
            {
                _normalizedPosition %= 1f;
            }
            else
            {
                _normalizedPosition = Mathf.Clamp01(_normalizedPosition);
            }

            SplineContainer.Spline.Evaluate(_normalizedPosition, out var pos, out var tangent, out _);

            transform.position = (Vector3)pos + _offset;

            if (_lookForward)
            {
                transform.rotation = Quaternion.LookRotation(tangent);
            }
        }

        private void ExecuteFollowTarget()
        {
            if (FollowTransform == null)
            {
                return;
            }

            _currentYaw += _cameraInput.x * _rotationSpeed * Time.deltaTime;

            Vector3 rotationOffset = Quaternion.Euler(0, _currentYaw, 0) * Vector3.back * _orbitRadius;
            Vector3 verticalOffset = Vector3.up * _heightOffset;

            Vector3 desiredPosition = FollowTransform.position + rotationOffset + verticalOffset;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, _smoothTime);

            Vector3 lookTarget = FollowTransform.position + Vector3.up * (_heightOffset / 2);
            transform.LookAt(lookTarget);
        }
    }
}
