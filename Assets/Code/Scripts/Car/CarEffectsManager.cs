using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using XaviGames.Ui;

namespace XaviGames.Car
{
    public class CarEffectsManager : NetworkBehaviour
    {
        [Header("Trail Renderers")]
        [SerializeField]
        private List<TrailRenderer> _wheelTrailRenderers;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                SetUiButtonReferences();
            }
        }

        private void SetUiButtonReferences()
        {
            if (!IsOwner)
            {
                return;
            }

            HudController hud = CanvasManager.Instance.HudController;

            AddTriggerEvent(hud.HandbrakeButton, EventTriggerType.PointerDown, () => SetTrailsActiveServerRpc(true));
            AddTriggerEvent(hud.HandbrakeButton, EventTriggerType.PointerUp, () => SetTrailsActiveServerRpc(false));
        }

        private void AddTriggerEvent(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction action)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = eventType
            };
            entry.callback.AddListener((_) => action.Invoke());
            trigger.triggers.Add(entry);
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
