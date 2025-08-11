using UnityEngine;
using XaviGames.Host;
using XaviGames.Manager;

namespace XaviGames.Ui
{
    public class WaitingPlayersBannerController : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroupController _canvasGroupController;

        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        //private void Update()
        //{

        //    if (_gameManager.GameState == GameState.WaitingForPlayers)
        //    {
        //        _canvasGroupController.EnableCanvas();
        //    }
        //    else
        //    {
        //        _canvasGroupController.DisableCanvas();
        //    }
        //}

    }
}
