using UnityEngine;

namespace XaviGames.Manager
{
    public class MenuManager : MonoBehaviour
    {
        public async void StartMatch()
        {
            await ClientManager.Instance.StartSearch();
        }
    }
}
