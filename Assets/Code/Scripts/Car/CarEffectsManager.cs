using System.Collections.Generic;
using UnityEngine;
using XaviEssencials.Runtime;

namespace XaviGames.Car
{
    public class CarEffectsManager : MonoBehaviour
    {
        [Header("Trail Renderers")]
        [SerializeField] 
        private List<TrailRenderer> _wheelTrailRenderers;

        [Header("Event Channels")]
        [SerializeField] 
        private EventChannel _onCarDrifting;

        private void OnEnable()
        {
            _onCarDrifting.OnEventRaisedWithContext += OnCarDrifting;
        }

        private void OnDisable()
        {
            _onCarDrifting.OnEventRaisedWithContext -= OnCarDrifting;
        }

        private void OnCarDrifting(object state)
        {
            if (state is bool isDrifting)
            {
                foreach (var trail in _wheelTrailRenderers)
                {
                    trail.emitting = isDrifting;
                }
            }
        }
    }
}
