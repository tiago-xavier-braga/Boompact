//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using System.Collections;
using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Host;

namespace XaviGames.Car
{
    public class CarBombHandler : NetworkBehaviour
    {
        public NetworkVariable<bool> HasBomb = new NetworkVariable<bool>
            (
                false,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Owner
            );

        public NetworkVariable<bool> CanTransferBomb = new NetworkVariable<bool>
    (
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

        [SerializeField]
        private CanvasGroup _bombCanvas;

        [SerializeField]
        private float _bombCanvasOwnerAlpha = 0.5f;

        [SerializeField]
        private float _bombCanvasOtherAlpha = 1f;

        [SerializeField]
        private float _transferCooldown = 2f;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!HasBomb.Value)
            {
                return;
            }

            if (!CanTransferBomb.Value)
            {
                return;
            }

            CarBombHandler otherHandler = other.GetComponent<CarBombHandler>();
            if (otherHandler == null)
            {
                return;
            }

            if (otherHandler.HasBomb.Value)
            {
                return;
            }

            ulong toClientId = otherHandler.OwnerClientId;

            TransferBombServerRpc(toClientId);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void GiveBombRpc()
        {
            if (IsOwner)
            {
                HasBomb.Value = true;
                _bombCanvas.alpha = _bombCanvasOwnerAlpha;
                CanTransferBomb.Value = false;
                StartCoroutine(Countdown());
            }
            else
            {
                _bombCanvas.alpha = _bombCanvasOtherAlpha;
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void RemoveBombRpc()
        {
            if (IsOwner)
            {
                HasBomb.Value = false;
                CanTransferBomb.Value = true;
            }

            _bombCanvas.alpha = 0f;
        }

        [Rpc(SendTo.Server)]
        private void TransferBombServerRpc(ulong toClientId)
        {
            var teamController = FindAnyObjectByType<TeamController>();
            if (teamController == null)
            {
                GameLogger.LogError("TeamController not found in the scene.", LogCategory.Server);
                return;
            }

            teamController.TransferBombBetweenPlayers(OwnerClientId, toClientId);
        }

        private IEnumerator Countdown()
        {
            yield return new WaitForSeconds(_transferCooldown);
            CanTransferBomb.Value = true;
        }
    }
}
