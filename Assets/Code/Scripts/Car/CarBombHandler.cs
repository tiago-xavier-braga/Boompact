//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using Unity.Netcode;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Server;

namespace XaviGames.Car
{
    public class CarBombHandler : NetworkBehaviour
    {
        [field: SerializeField]
        [field: ReadOnly]
        public bool HasBomb { get; private set; } = false;

        [SerializeField]
        private CanvasGroup _bombCanvas;

        private void OnTriggerEnter(Collider other)
        {
            if (!HasBomb)
            {
                return;
            }

            CarBombHandler otherHandler = other.GetComponent<CarBombHandler>();
            if (otherHandler == null)
            {
                return;
            }

            if (otherHandler.HasBomb)
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
            HasBomb = true;

            if (IsOwner)
            {
                _bombCanvas.alpha = 0.5f;
            }
            else
            {
                _bombCanvas.alpha = 1f;
            }

        }

        public void RemoveBomb()
        {
            HasBomb = false;
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
