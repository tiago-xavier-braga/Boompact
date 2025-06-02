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

        public void GiveBomb()
        {
            HasBomb = true;
            GiveBombServerRpc();
            GameLogger.Log($"Bomb given to {OwnerClientId}", LogCategory.Server);
        }

        [ServerRpc(RequireOwnership = false)]
        private void GiveBombServerRpc(ServerRpcParams rpcParams = default)
        {
            ShowBombClientRpc();
        }


        [ClientRpc]
        private void ShowBombClientRpc(ClientRpcParams rpcParams = default)
        {
            if (_bombCanvas == null)
            {
                return;
            }

            _bombCanvas.gameObject.SetActive(true);

            LeanTween.alphaCanvas(_bombCanvas, 1f, 0.5f)
                    .setEase(LeanTweenType.easeInOutQuad);
        }

        public void RemoveBomb()
        {
            HasBomb = false;
            RemoveBombServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void RemoveBombServerRpc(ServerRpcParams rpcParams = default)
        {
            RemoveBombClientRpc();
        }

        [ClientRpc]
        private void RemoveBombClientRpc(ClientRpcParams rpcParams = default)
        {
            if (_bombCanvas == null)
            {
                return;
            }

            LeanTween.alphaCanvas(_bombCanvas, 0f, 0.5f)
                    .setEase(LeanTweenType.easeInOutQuad);
        }

        [ServerRpc(RequireOwnership = false)]
        private void TransferBombServerRpc(ulong fromClientId, ulong toClientId)
        {
            var teamController = FindAnyObjectByType<TeamController>();
            if (teamController == null)
            {
                GameLogger.LogError("TeamController not found in the scene.", LogCategory.Server);
                return;
            }

            teamController.TransferBomb(fromClientId, toClientId);
        }
    }
}
