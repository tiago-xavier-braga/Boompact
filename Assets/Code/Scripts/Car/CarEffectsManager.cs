using System.Collections.Generic;
using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Car
{
    public class CarEffectsManager : MonoBehaviour
    {
        [Header("Trail Renderers")]
        [SerializeField] private List<TrailRenderer> _wheelTrailRenderers;

        [Header("Event Channels")]
        [SerializeField] private BoolEventChannel _onCarDrifting;

        private void OnEnable()
        {
            _onCarDrifting.OnEventRaised += OnCarDrifting;
        }

        private void OnDisable()
        {
            _onCarDrifting.OnEventRaised -= OnCarDrifting;
        }

        private void OnCarDrifting(bool state)
        {
            foreach (var trail in _wheelTrailRenderers)
            {
                trail.emitting = state;
            }
        }
    }
}
