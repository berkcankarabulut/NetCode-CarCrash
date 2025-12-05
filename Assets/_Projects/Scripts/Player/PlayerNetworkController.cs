 

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
        [SerializeField] private CinemachineCamera _playerCamera; 
        [SerializeField] private TMP_Text _playerName; 
        private PlayerVehicleController _playerVehicleController;
        private PlayerSkillController _playerSkillController;
        private PlayerInteractionController _playerInteractionController;
        
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
            }
            
            if(!IsOwner) return;
            _playerVehicleController = GetComponent<PlayerVehicleController>();
            _playerSkillController = GetComponent<PlayerSkillController>();
            _playerInteractionController = GetComponent<PlayerInteractionController>();
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