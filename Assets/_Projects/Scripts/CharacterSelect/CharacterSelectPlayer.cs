using _Projects.Networking;
using _Projects.Networking.Host;
using _Projects.Networking.Server;
using _Projects.Scripts.Serializables;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Projects.CharacterSelect
{
    public class CharacterSelectPlayer : NetworkBehaviour
    {
        [SerializeField] private int _playerIndex;
        [SerializeField] private TMP_Text _playerNameText;
        [SerializeField] private GameObject _readyGameObject;
        [SerializeField] private CharacterSelectVisual _characterSelectVisual;
        [SerializeField] private Button _kickButton;

        public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

        private void Awake()
        {
            _kickButton.onClick.AddListener(OnKickButtonClicked);
        }

        private void Start()
        {
            MultiplayerManager.Instance.OnPlayerDataNetworkListChanged
                += OnPlayerDataNetworkListChanged;
            CharacterSelectReady.Instance.OnReadyChanged += OnReadyChanged;

            UpdatePlayer();

            HandlePlayerNameChanged(string.Empty, PlayerName.Value);
            PlayerName.OnValueChanged += HandlePlayerNameChanged;
        }

        private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
        {
            _playerNameText.text = newName.ToString();
        }

        private void OnKickButtonClicked()
        {
            PlayerDataSerializable playerData =
                MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
            MultiplayerManager.Instance.KickPlayer(playerData.ClientId);
        }

        private void OnReadyChanged()
        {
            UpdatePlayer();
        }

        private void OnPlayerDataNetworkListChanged()
        {
            UpdatePlayer();
        }

        private void UpdatePlayer()
        {
            if (MultiplayerManager.Instance.IsPlayerIndexConnected(_playerIndex))
            {
                Show();

                PlayerDataSerializable playerData
                    = MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
                _readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.ClientId));
                _characterSelectVisual.SetPlayerColor(
                    MultiplayerManager.Instance.GetPlayerColor(playerData.ColorId));
                HideKickButtonOnHost(playerData);
                SetOwner(playerData.ClientId);
                UpdatePlayerNameRpc();
            }
            else
            {
                Hide();
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void HideKickButtonOnHost(PlayerDataSerializable playerData)
        {
            _kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer &&
                                             playerData.ClientId != NetworkManager.Singleton.LocalClientId);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void UpdatePlayerNameRpc()
        {
            if (IsServer)
            {
                UserData userData =
                    HostSingleton.Instance.HostManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

                if (userData != null)
                { 
                    PlayerName.Value = userData.UserName;
                }
                else
                {
                    Debug.LogWarning($"UserData not found for OwnerClientId: {OwnerClientId}");
                }
            }
        }

        private void SetOwner(ulong clientId)
        {
            if (IsServer)
            {
                var networkObject = GetComponent<NetworkObject>();

                if (networkObject.OwnerClientId != clientId)
                {
                    networkObject.ChangeOwnership(clientId);
                }
            }
        }

        public override void OnDestroy()
        {
            MultiplayerManager.Instance.OnPlayerDataNetworkListChanged
                -= OnPlayerDataNetworkListChanged;
            CharacterSelectReady.Instance.OnReadyChanged -= OnReadyChanged;
            PlayerName.OnValueChanged -= HandlePlayerNameChanged;
        }
    }
}