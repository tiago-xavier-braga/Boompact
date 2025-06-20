using UnityEngine;
using UnityEngine.InputSystem;
using XaviEssencials.Runtime;
using XaviGames.Ui;

namespace XaviGames.Camera
{
    public class GhostCamera : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference _moveInputAction;

        [SerializeField]
        private InputActionReference _lookInputAction;

        [SerializeField]
        private float _moveSpeed = 5f;

        [SerializeField]
        private float _lookSpeed = 2f;

        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _inputValue = Vector2.zero;

        [SerializeField]
        [ReadOnly]
        private Vector2 _rotateValue = Vector2.zero;

        private float _pitch = 0f;
        private float _yaw = 0f;

        private void OnEnable()
        {
            _moveInputAction.action.performed += OnMoveInput;
            _moveInputAction.action.canceled += OnMoveInput;
            _lookInputAction.action.performed += OnLookInput;
            _lookInputAction.action.canceled += OnLookInput;
        }

        private void OnDisable()
        {
            _moveInputAction.action.performed -= OnMoveInput;
            _moveInputAction.action.canceled -= OnMoveInput;
            _lookInputAction.action.performed -= OnLookInput;
            _lookInputAction.action.canceled -= OnLookInput;
        }

        private void Start()
        {
            CanvasManager.Instance.LoadingCanvasController.DisableLoading();
            Vector3 angles = transform.eulerAngles;
            _yaw = angles.y;
            _pitch = angles.x;
        }

        private void FixedUpdate()
        {
            Vector3 moveDirection = (transform.forward * _inputValue.y) + (transform.right * _inputValue.x);

            transform.position += moveDirection * _moveSpeed * Time.fixedDeltaTime;

            _yaw += _rotateValue.x * _lookSpeed * Time.fixedDeltaTime;
            _pitch -= _rotateValue.y * _lookSpeed * Time.fixedDeltaTime;
            _pitch = Mathf.Clamp(_pitch, -89f, 89f);

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0);
        }

        private void OnMoveInput(InputAction.CallbackContext context)
        {
            _inputValue = context.ReadValue<Vector2>();
        }

        private void OnLookInput(InputAction.CallbackContext context)
        {
            _rotateValue = context.ReadValue<Vector2>();
        }
    }
}
