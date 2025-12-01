using System;
using _Project.Collect;
using _Project.UI.Network;
using Unity.Netcode;
using UnityEngine;

namespace Game.Player
{
    public class PlayerInteractionController : NetworkBehaviour
    {
        private PlayerVehicleController _vehicleController;
        private PlayerSkillController _playerSkillController;  
        private bool _isCrash = false;
        private bool _isShieldActive = false;
        private bool _iSpikeActive = false;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsOwner) return;
            _playerSkillController = GetComponent<PlayerSkillController>();
            _vehicleController = GetComponent<PlayerVehicleController>(); 
            _vehicleController.OnVehicleCrashed += VehicleCrash;
        }

        private void VehicleCrash()
        {
            enabled = false;
            _isCrash = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckCollision(other);
        }

        private void OnTriggerStay(Collider other)
        {
            CheckCollision(other);
        }

        private void CheckCollision(Collider other)
        {
            if (!IsOwner || _isCrash) return;

            if (other.gameObject.TryGetComponent(out ICollectible collectible))
            {
                collectible.Collect(_playerSkillController);
            }
            else
                DamageToTarget(other);
        }

        private void DamageToTarget(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out IDamageable damageable)) return;
            if (_isShieldActive)
            {
                Debug.Log($"Shield Active");
                return;
            }
            damageable.Damage(_vehicleController);
            SetKillerUIRPC(damageable.GetKillerClientID(),
                RpcTarget.Single(damageable.GetKillerClientID(), RpcTargetUse.Temp));
        }

        [Rpc(SendTo.SpecifiedInParams)]
        private void SetKillerUIRPC(ulong killerID, RpcParams rpcParams)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(killerID,out var killerClient))
            {
                KillScreenUI.Instance.SetSmashUI("Berkcan");
            }
        }
        
        public void SetShieldActive(bool active) => _isShieldActive = active;
        public void SetSpikeActive(bool active) => _iSpikeActive = active;
    }
}