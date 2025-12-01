using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Game.Player
{
    public class PlayerNetworkController : NetworkBehaviour
    {
        [SerializeField] private CinemachineCamera _playerCamera; 
        private PlayerVehicleController _playerVehicleController;
        private PlayerSkillController _playerSkillController;
        private PlayerInteractionController _playerInteractionController;
        
        public override void OnNetworkSpawn()
        {
            _playerCamera.gameObject.SetActive(IsOwner);
            
            if(!IsOwner) return;
            _playerVehicleController = GetComponent<PlayerVehicleController>();
            _playerSkillController = GetComponent<PlayerSkillController>();
            _playerInteractionController = GetComponent<PlayerInteractionController>();
        }

        public void OnPlayerRespawn()
        {
            _playerInteractionController.OnPlayerRespawn();
            _playerSkillController.OnPlayerRespawn();
            _playerVehicleController.OnPlayerRespawned();
        }
    }
}