using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XaviGames.Car
{
    public class CarEffectsManager : NetworkBehaviour
    {
        [Header("Trail Renderers")]
        [SerializeField]
        private List<TrailRenderer> _wheelTrailRenderers;

        [Header("Input Actions")]
        [SerializeField]
        private InputActionReference _handbrakeInputAction;

        private void OnEnable()
        {
            _handbrakeInputAction.action.performed += OnCarDrifting;
            _handbrakeInputAction.action.canceled += OnCarDrifting;
        }

        private void OnDisable()
        {
            _handbrakeInputAction.action.performed -= OnCarDrifting;
            _handbrakeInputAction.action.canceled -= OnCarDrifting;
        }

        private void OnCarDrifting(InputAction.CallbackContext context)
        {
            if (!IsOwner)
            {
                return;
            }

            bool isDrifting = context.phase == InputActionPhase.Performed;
            SetTrailsActiveServerRpc(isDrifting);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetTrailsActiveServerRpc(bool isActive, ServerRpcParams rpcParams = default)
        {
            UpdateTrailsClientRpc(isActive);
        }

        [ClientRpc]
        private void UpdateTrailsClientRpc(bool isActive, ClientRpcParams clientRpcParams = default)
        {
            foreach (var trail in _wheelTrailRenderers)
            {
                trail.emitting = isActive;
            }
        }
    }
}