using UnityEngine;
using XaviGames.Manager;

namespace XaviGames.Ui
{
    public class MenuManager : MonoBehaviour
    {
        public async void StartMatch()
        {
            await ClientManager.Instance.StartSearch();
        }
    }
}
