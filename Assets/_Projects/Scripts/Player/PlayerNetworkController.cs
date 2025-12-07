using System;
using _Projects.Networking.Host;
using _Projects.Networking.Server;
using TMPro;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.Player
{
    public class PlayerNetworkController : NetworkBehaviour
    {
        public static event Action<PlayerNetworkController> OnPlayerSpawned;
        public static event Action<PlayerNetworkController> OnPlayerDespawned;
        [Header("Components")]
        [SerializeField] private CinemachineCamera _playerCamera;
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private PlayerVehicleController _playerVehicleController;
        [SerializeField] private PlayerSkillController _playerSkillController;
        [SerializeField] private PlayerInteractionController _playerInteractionController;
        [SerializeField] private PlayerScoreController _playerScoreController;
        public PlayerScoreController PlayerScoreController => _playerScoreController;
        public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

        public override void OnNetworkSpawn()
        {
            _playerCamera.gameObject.SetActive(IsOwner);
            if (IsServer)
            {
                UserData userData =
                    HostSingleton.Instance.HostManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
                PlayerName.Value = userData.UserName;
                SetPlayerNameRPC();
                OnPlayerSpawned?.Invoke(this);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            OnPlayerDespawned?.Invoke(this);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SetPlayerNameRPC()
        {
            _playerName.text = PlayerName.Value.ToString();
        }

        public void OnPlayerRespawn()
        {
            _playerInteractionController.OnPlayerRespawn();
            _playerSkillController.OnPlayerRespawn();
            _playerVehicleController.OnPlayerRespawned();
        }
    }
}