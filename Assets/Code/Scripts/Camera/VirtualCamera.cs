using System;
using UnityEngine;
using UnityEngine.InputSystem;
using XaviEssencials.Runtime;

namespace XaviGames.Cameras
{
    public class VirtualCamera : MonoBehaviour
    {
        [SerializeField]
        private CameraOption cameraOption;

        [field: SerializeField]
        public Transform FollowTransform { get; private set; }

        [field: SerializeField]
        public float RadiusCircle;

        [Header("Camera Settings")]
        [SerializeField] 
        private float _heightOffset = 3f;
       
        [SerializeField] 
        private float _rotationSpeed = 120f;
        
        [SerializeField] 
        private float _smoothTime = 0.1f;

        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _inputValue;

        private Vector3 _currentVelocity;
        private float _currentYaw;


        private void Update()
        {
            switch (cameraOption)
            {
                case CameraOption.Fixed:
                    break;
                case CameraOption.FollowSpline:
                    break;
                case CameraOption.FollowTarget:
                    ExecuteFollowTarget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnCameraInput(InputAction.CallbackContext context)
        {
            _inputValue = context.ReadValue<Vector2>();
        }

        public void SetFollowTransform(Transform transform)
        {
            if (transform is null)
            {
                GameLogger.LogError("Transform is null", LogCategory.Unity);
                return;
            }

            FollowTransform = transform;
        }

        private void ExecuteFollowTarget()
        {
            if (FollowTransform == null)
            {
                return;
            }

            _currentYaw += _inputValue.x * _rotationSpeed * Time.deltaTime;

            Vector3 offset = Quaternion.Euler(0, _currentYaw, 0) * Vector3.back * RadiusCircle;
            Vector3 desiredPosition = FollowTransform.position + offset + Vector3.up * _heightOffset;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, _smoothTime);

            transform.LookAt(FollowTransform.position + Vector3.up * (_heightOffset / 2));
        }
    }
}

