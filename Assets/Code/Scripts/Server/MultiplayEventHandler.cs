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
            GameLogger.LogWarning($"Subscription state changed to: {state}", LogCategory.Server);
        }

        public void OnServerErrorReceived(MultiplayError error)
        {
            GameLogger.LogError($"Error received. {error}", LogCategory.Server);
        }

        public async void OnServerDeallocated(MultiplayDeallocation deallocation)
        {
            GameLogger.LogWarning($"Deallocation received. ServerId: {deallocation.ServerId}, EventId: {deallocation.EventId}", LogCategory.Server);

            if (_servicesSettings.BuildServiceType != ServiceType.Local)
            {
                await MultiplayService.Instance.UnreadyServerAsync();
            }
        }

        public async void OnServerAllocated(MultiplayAllocation allocation)
        {
            GameLogger.LogWarning($"Allocation received. AllocationId: {allocation.AllocationId}", LogCategory.Server);

            if (_servicesSettings.BuildServiceType != ServiceType.Local)
            {
                await MultiplayService.Instance.ReadyServerForPlayersAsync();
            }
        }
    }
}
#endif