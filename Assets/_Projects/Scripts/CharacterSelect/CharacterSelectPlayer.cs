using System;
using _Projects.Networking;
using _Projects.Scripts.Serializables;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Projects.CharacterSelect
{
    public class CharacterSelectPlayer : NetworkBehaviour
    {
        [SerializeField] private int _playerIndex;
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private GameObject _readyGameObject;
        [SerializeField] private Button _kickButton;

        private void Start()
        {
            MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += OnPlayerDataNetworkChanged;
            CharacterSelectReady.Instance.OnReadyChanged += OnPlayerDataNetworkChanged;
            UpdatePlayer();
        }

        private void OnPlayerDataNetworkChanged()
        {
            UpdatePlayer();
        }

        private void UpdatePlayer()
        {
            if (MultiplayerManager.Instance.IsPlayerIndexConnected(_playerIndex))
            {
                gameObject.SetActive(true); 
                PlayerDataSerializable playerData = MultiplayerManager.Instance.GetPlayerData(_playerIndex);
                _readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.ClientId));
                HideKickButton(playerData);
            }
            else gameObject.SetActive(false);
        }

        private void HideKickButton(PlayerDataSerializable playerData)
        {
            _kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer &&
                                             playerData.ClientId != NetworkManager.Singleton.LocalClientId);
        }
    }
}