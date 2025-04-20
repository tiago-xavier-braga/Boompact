using UnityEngine;
using UnityEngine.InputSystem;
using XaviEssencials.Runtime;

namespace XaviGames.Cameras
{
    public class CarFollowCamera : VirtualCamera
    {
        [Header("Input Settings")]
        [SerializeField]
        private InputActionReference _cameraInputAction;

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

        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _cameraInput;

        private Vector3 _currentVelocity;
        private float _currentYaw;

        private void OnEnable()
        {
            _cameraInputAction.action.performed += OnCameraInput;
            _cameraInputAction.action.canceled += OnCameraInput;
        }

        private void OnDisable()
        {
            _cameraInputAction.action.performed -= OnCameraInput;
            _cameraInputAction.action.canceled -= OnCameraInput;
        }

        private void LateUpdate()
        {
            if (FollowTransform == null)
            {
                GameLogger.LogWarning("FollowTransform is null", LogCategory.Unity);
                return;
            }
            ExecuteFollowTarget();
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