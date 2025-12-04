using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace _Projects.Scripts.UI.MainMenu
{
    public class LobbyItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _lobbyNameText;

        [SerializeField] private TMP_Text _lobbyPlayersText;

        [SerializeField] private Button _joinButton;
        private LobbiesListUI _lobbiesList;
        private Lobby _lobby;

        private void OnEnable()
        {
            _joinButton.onClick.AddListener(OnJoinClicked);
        }

        private void OnDisable()
        {
            _joinButton.onClick.RemoveAllListeners();
        }

        public void Setup(LobbiesListUI lobbiesList, Lobby lobby)
        {
            _lobbiesList = lobbiesList;
            _lobby = lobby;
            _lobbyNameText.text = lobby.Name;
            _lobbyPlayersText.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";
        }

        private void OnJoinClicked()
        {
            _lobbiesList.JoinAsync(_lobby);
        }
    }
}