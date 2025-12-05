using _Projects.Helpers.Const;
using _Projects.Networking.Client;
using _Projects.Networking.Host;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Projects.Scripts.UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private LobbiesListUI _lobbiesListUI;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;
        [SerializeField] private Button _lobbiesButton;
        [SerializeField] private Button _closeLobbyButton;
        [SerializeField] private Button _refreshButton;
        [SerializeField] private RectTransform _lobbiesTransform;
        [SerializeField] private TMP_InputField _hostInputField;
        [SerializeField] private TMP_Text _welcomeText;

        [SerializeField] private float _animationDuration = 0.5f;

        private void OnEnable()
        {
            var playerName = PlayerPrefs.GetString(PlayerData.PLAYER_NAME, string.Empty);
            _welcomeText.text = $"welcome, <color=yellow>{playerName}</color>";
            _hostButton.onClick.AddListener(StartHost);
            _clientButton.onClick.AddListener(StartClient);
            _lobbiesButton.onClick.AddListener(OpenLobbies);
            _closeLobbyButton.onClick.AddListener(CloseLobbies);
            _refreshButton.onClick.AddListener(RefreshLobbies);
        }

        private void OnDisable()
        {
            _hostButton.onClick.RemoveListener(StartHost);
            _clientButton.onClick.RemoveListener(StartClient);
            _lobbiesButton.onClick.RemoveListener(OpenLobbies);
            _closeLobbyButton.onClick.RemoveListener(CloseLobbies);
            _refreshButton.onClick.RemoveListener(RefreshLobbies);
        }

        private async void StartHost()
        {
            _hostButton.interactable = false;
            await HostSingleton.Instance.HostManager.StartHostAsync();
        }

        private async void StartClient()
        {
            await ClientSingleton.Instance.ClientManager.StartClientAsync(_hostInputField.text);
        }

        private void OpenLobbies()
        {
            _lobbiesTransform.DOAnchorPosX(-650f, _animationDuration).SetEase(Ease.OutBack);
            _lobbiesListUI.RefreshLobby();
        }

        private void CloseLobbies()
        {
            _lobbiesTransform.DOAnchorPosX(900, _animationDuration).SetEase(Ease.OutBack);
        }

        private void RefreshLobbies()
        {
            _lobbiesListUI.RefreshLobby();
        }
    }
}