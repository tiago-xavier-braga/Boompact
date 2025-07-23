using UnityEngine;
using UnityEngine.InputSystem;
using XaviEssencials.Runtime;
using XaviGames.Audio;
using XaviGames.Host;

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
        private float _idlePitch = 0.7f;

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

        private void Update()
        {
            if (HostManager.Instance.HostState != HostState.GameInProgress)
            {
                if (_idleAudioSource.isPlaying)
                {
                    _idleAudioSource.Stop();
                }

                if (_movingAudioSource.isPlaying)
                {
                    _movingAudioSource.Stop();
                }
                
                return;
            }

            if (_carMovementController.KmPerHour <= 1f)
            {
                if (!_idleAudioSource.isPlaying)
                {
                    _idleAudioSource.clip = _idleClip;
                    _idleAudioSource.pitch = _idlePitch;
                    _idleAudioSource.volume = _idleVolume;
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
                    _movingAudioSource.volume = _movingVolume;
                    _movingAudioSource.Play();
                }
                float pitch = Mathf.Lerp(_minMovingPitch, _maxMovingPitch, 
                                         Mathf.InverseLerp(0f, 100f, _carMovementController.KmPerHour));
                _movingAudioSource.pitch = pitch;
            }
        }
    }
}
