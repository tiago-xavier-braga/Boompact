using UnityEngine;
using UnityEngine.InputSystem;
using XaviEssencials.Runtime;
using XaviGames.Audio;

namespace XaviGames.Car
{
    public class CarSoundController : MonoBehaviour
    {
        [SerializeField]
        private CarMovementController _carMovementController;

        [Header("Audio Sources References")]
        [SerializeField]
        private AudioSource _idleAudioSource;

        [SerializeField]
        private AudioSource _movingAudioSource;

        [Header("Clips References")]
        [SerializeField]
        private AudioClip _idleClip;

        [SerializeField]
        [Range(0, 1f)]
        private float _idleVolume = 0.5f;

        [SerializeField]
        private AudioClip _movingClip;

        [SerializeField]
        [Range(0, 1f)]
        private float _movingVolume = 0.5f;

        [SerializeField]
        private float _minMovingPitch = 0.8f;

        [SerializeField]
        private float _maxMovingPitch = 1.2f;

        [Header("Input References")]
        [SerializeField]
        private InputActionReference _moveInputAction;

        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private float _currentIdleVolume = 0f;

        [SerializeField]
        [ReadOnly]
        private float _currentMovingVolume;

        private void OnEnable()
        {
            _moveInputAction.action.performed += OnMoveInput;
            _moveInputAction.action.canceled += OnMoveInput;
            AudioManager.Instance.OnMasterVolumeChanged += HandleMasterVolumeChanged;
        }

        private void OnDisable()
        {
            _moveInputAction.action.performed -= OnMoveInput;
            _moveInputAction.action.canceled -= OnMoveInput;
            AudioManager.Instance.OnMasterVolumeChanged -= HandleMasterVolumeChanged;
        }

        private void Start()
        {
            HandleMasterVolumeChanged(AudioManager.Instance.MasterVolume);
        }

        private void Update()
        {
            if (_carMovementController._kmPerHour <= 1f)
            {
                if (!_idleAudioSource.isPlaying)
                {
                    _idleAudioSource.clip = _idleClip;
                    _idleAudioSource.volume = _currentIdleVolume;
                    _idleAudioSource.Play();
                }
                if (_movingAudioSource.isPlaying)
                {
                    _movingAudioSource.Stop();
                }
            }
            else
            {
                if (!_movingAudioSource.isPlaying)
                {
                    _movingAudioSource.clip = _movingClip;
                    _movingAudioSource.volume = _currentMovingVolume;
                    _movingAudioSource.Play();
                }
                float pitch = Mathf.Lerp(_minMovingPitch, _maxMovingPitch, 
                                         Mathf.InverseLerp(0f, 100f, _carMovementController._kmPerHour));
                _movingAudioSource.pitch = pitch;
            }
        }

        private void OnMoveInput(InputAction.CallbackContext context)
        {
            
        }

        private void HandleMasterVolumeChanged(float value)
        {
            _currentIdleVolume = Mathf.Lerp(0f, _idleVolume, value);
            _currentMovingVolume = Mathf.Lerp(0f, _movingVolume, value);
        }
    }
}
