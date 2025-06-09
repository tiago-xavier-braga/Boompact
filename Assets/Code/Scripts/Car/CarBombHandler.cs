//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

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

        [SerializeField]
        private CanvasGroup _bombCanvas;

        private void OnTriggerEnter(Collider other)
        {
            if (!HasBomb.Value)
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

            ulong fromClientId = OwnerClientId;
            ulong toClientId = otherHandler.OwnerClientId;

            TransferBombServerRpc(fromClientId, toClientId);
        }

        [Rpc(SendTo.NotServer)]
        public void GiveBombRpc()
        {
            if (IsOwner)
            {
                HasBomb.Value = true;
                //TODO: Add visual feedback for the player who received the bomb
                _bombCanvas.alpha = 0.5f;
            }
            else
            {
                _bombCanvas.alpha = 1f;
            }

        }

        [Rpc(SendTo.NotServer)]
        public void RemoveBombRpc()
        {
            if (IsOwner)
            {
                HasBomb.Value = false;
            }

            _bombCanvas.alpha = 0f;
        }

        [Rpc(SendTo.Server)]
        private void TransferBombServerRpc(ulong fromClientId, ulong toClientId)
        {
            var teamController = FindAnyObjectByType<TeamController>();
            if (teamController == null)
            {
                GameLogger.LogError("TeamController not found in the scene.", LogCategory.Server);
                return;
            }

            teamController.TransferBombBetweenPlayers(fromClientId, toClientId);
        }
    }
}
