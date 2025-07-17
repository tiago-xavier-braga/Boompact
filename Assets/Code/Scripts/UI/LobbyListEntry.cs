// Boompact (c) 2025 Tiago Xavier Braga - XaviGames. All rights reserved.
// Unauthorized use, copying, or distribution is prohibited.
// For inquiries: xavigames.company@gmail.com

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XaviEssencials.Runtime;

namespace XaviGames.Ui
{
    public class LobbyListEntry : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _relayTextMesh;

        [SerializeField]
        private TextMeshProUGUI _numberPlayersTextMesh;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private Image _buttonImage;

        [Header("Button Style")]
        [SerializeField]
        private Sprite _selectedButtonSprite;

        [SerializeField]
        private Sprite _unselectedButtonSprite;

        [Header("Info")]
        [SerializeField]
        [ReadOnly]
        private bool _isSelected = false;

        [field: SerializeField]
        [field: ReadOnly]
        public string RelayCode { get; private set; } = string.Empty;
        
        private MenuUIController _menuUiController;

        private void Start()
        {
            _menuUiController = MenuUIController.Instance;
            _button.onClick.AddListener(OnButtonClicked);
        }

        public void UnselectButton()
        {
            if (_isSelected)
            {
                _isSelected = false;
                _button.image.sprite = _unselectedButtonSprite;
            }
        }


        public void SetLobbyInfos(string relayCode, int currentCountPlayers, int maxCountPlayers)
        {
            if (_relayTextMesh != null)
            {
                RelayCode = relayCode;
                _relayTextMesh.text = relayCode;
            }
            else
            {
                GameLogger.LogWarning("Relay TextMeshProUGUI is not assigned.", LogCategory.Client);
            }

            if (_numberPlayersTextMesh != null)
            {
                _numberPlayersTextMesh.text = $"{currentCountPlayers}/{maxCountPlayers}";
            }
            else
            {
                GameLogger.LogWarning("Number of Players TextMeshProUGUI is not assigned.", LogCategory.Client);
            }
        }

        private void OnButtonClicked()
        {
            if (_isSelected)
            {
                _menuUiController.SetJoinCode(string.Empty);
                _button.image.sprite = _unselectedButtonSprite;
            }
            else
            {
                _menuUiController.SetJoinCode(RelayCode);
                _button.image.sprite = _selectedButtonSprite;
                _menuUiController.RoomConnectSelector.UnselectedAllButton();
            }

            _isSelected = !_isSelected;
        }
    }
}
