using System;
using System.Collections.Generic;
using _Project.Networking.Client;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI.MainMenu
{
    public class LobbiesListUI : MonoBehaviour
    {
        [SerializeField] private Transform _lobbyItemParent;
        [SerializeField] private LobbyItem _lobbyItemPrefab;
        [SerializeField] private Button _refreshButton;
        private bool _isJoiningLobby = false;
        private bool _isRefreshing = false;

        public async void JoinAsync(Lobby lobby)
        {
            if (_isJoiningLobby) return;
            try
            {
                Lobby joinLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
                string joinCode = joinLobby.Data["JoinCode"].Value;
                await ClientSingleton.Instance.ClientManager.StartClientAsync(joinCode);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }

            _isJoiningLobby = false;
        }

        public async void RefreshLobby()
        {
            if (_isRefreshing) return;
            _isRefreshing = true;
            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions();
                options.Count = 20;
                options.Filters = new List<QueryFilter>()
                {
                    new QueryFilter
                    (
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0"
                    )
                };
                
                QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);
                foreach (Transform child in _lobbyItemParent)
                {
                    Destroy(child.gameObject);
                }

                foreach (Lobby lobby in lobbies.Results)
                {
                    LobbyItem lobbyItem = Instantiate(_lobbyItemPrefab, _lobbyItemParent);
                    lobbyItem.Setup(this, lobby);
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }

            _isRefreshing = false;
        }
    }
}