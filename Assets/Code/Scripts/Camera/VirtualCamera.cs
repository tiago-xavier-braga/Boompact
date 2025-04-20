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
        [field: Header("Virtual Camera")]
        [field: SerializeField]
        public CameraOption CameraOption { get; private set; }

        public void SetCameraOption(CameraOption cameraOption)
        {
            CameraOption = cameraOption;
        }

        //[field: Header("Follow Spline Settings")]
        //[field: SerializeField]
        //public SplineContainer SplineContainer { get; private set; }

        //[Range(0f, 1f)]
        //[SerializeField]
        //private float _normalizedPosition;

        //[SerializeField]
        //private float _speed = 0.2f;

        //[SerializeField]
        //private bool _loop = false;

        //[SerializeField]
        //private bool _lookForward = true;

        //[SerializeField]
        //private Vector3 _offset;

        //private float _splineLength;

        //private void Start()
        //{
        //    if (SplineContainer != null)
        //    {
        //        UpdateSplineLength();
        //    }
        //}

        //public void SetSplineContainer(SplineContainer splineContainer)
        //{
        //    if (splineContainer == null)
        //    {
        //        GameLogger.LogError("SplineContainer is null", LogCategory.Unity);
        //        return;
        //    }

        //    SplineContainer = splineContainer;
        //    UpdateSplineLength();
        //}

        //private void UpdateSplineLength()
        //{
        //    _splineLength = SplineContainer.Spline.GetLength();
        //}

        //private void ExecuteFollowSpline()
        //{
        //    _normalizedPosition += _speed * Time.deltaTime / _splineLength;

        //    if (_loop)
        //    {
        //        _normalizedPosition %= 1f;
        //    }
        //    else
        //    {
        //        _normalizedPosition = Mathf.Clamp01(_normalizedPosition);
        //    }

        //    SplineContainer.Spline.Evaluate(_normalizedPosition, out var pos, out var tangent, out _);

        //    transform.position = (Vector3)pos + _offset;

        //    if (_lookForward)
        //    {
        //        transform.rotation = Quaternion.LookRotation(tangent);
        //    }
        //}

    }
}
