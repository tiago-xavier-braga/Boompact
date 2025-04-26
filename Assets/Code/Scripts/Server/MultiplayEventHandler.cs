//Boompact(c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

#if UNITY_SERVER
using Unity.Services.Multiplay;
using UnityEngine;
using XaviEssencials.Runtime;
using XaviGames.Services;

namespace XaviGames.Server
{
    public class MultiplayEventHandler : MonoBehaviour
    {
        [SerializeField]
        private ServicesSettings _servicesSettings;

        public void OnServerSubscriptionStateChanged(MultiplayServerSubscriptionState state)
        {
            GameLogger.LogWarning($"[Multiplay] Subscription state changed to: {state}", LogCategory.Server);
        }

        public void OnServerErrorReceived(MultiplayError error)
        {
            GameLogger.LogError($"[Multiplay] Error received. Status: {error.Status}, Message: {error.Message}", LogCategory.Server);
        }

        public async void OnServerDeallocated(MultiplayDeallocation deallocation)
        {
            GameLogger.LogWarning($"[Multiplay] Deallocation received. ServerId: {deallocation.ServerId}, EventId: {deallocation.EventId}", LogCategory.Server);

            if (_servicesSettings.BuildServiceType != ServiceType.Local)
            {
                await MultiplayService.Instance.UnreadyServerAsync();
            }
        }

        public async void OnServerAllocated(MultiplayAllocation allocation)
        {
            GameLogger.LogWarning($"[Multiplay] Allocation received. AllocationId: {allocation.AllocationId}, SessionId: {allocation.SessionId}", LogCategory.Server);

            if (_servicesSettings.BuildServiceType != ServiceType.Local)
            {
                await MultiplayService.Instance.ReadyServerForPlayersAsync();
            }
        }
    }
}
#endif