using _Projects.Collect;
using _Projects.Scripts.UI.GameUIManagement;
using _Projects.GameManagement;
using _Projects.SpawnSystem;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.Player
{
    public class PlayerInteractionController : NetworkBehaviour
    {
        private PlayerVehicleController _vehicleController;
        private PlayerSkillController _playerSkillController;
        private PlayerHealthController _playerHealthController;
        private PlayerNetworkController _playerNetworkController;
        private bool _isCrash = false;
        private bool _isShieldActive = false;
        private bool _iSpikeActive = false;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsOwner) return;
            _playerSkillController = GetComponent<PlayerSkillController>();
            _vehicleController = GetComponent<PlayerVehicleController>();
            _playerHealthController = GetComponent<PlayerHealthController>();
            _playerNetworkController = GetComponent<PlayerNetworkController>();
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
            if (GameManager.Instance.GameState != GameState.Playing) return;

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

            CrashVehicle(damageable);
        }

        private void CrashVehicle(IDamageable damageable)
        {
            var playerName = _playerNetworkController.PlayerName.Value;
            damageable.Damage(_vehicleController, damageable.GetKillerName());
            _playerHealthController.TakeDamage(damageable.GetDamageAmount);
            SetKillerUIRPC(damageable.GetKillerClientID(), playerName.ToString(),
                RpcTarget.Single(damageable.GetKillerClientID(), RpcTargetUse.Temp));
            SpawnManager.Instance.RespawnPlayer(damageable.GetRespawnTimer, OwnerClientId);
        }

        [Rpc(SendTo.SpecifiedInParams)]
        private void SetKillerUIRPC(ulong killerID, FixedString32Bytes playerName, RpcParams rpcParams)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(killerID, out var killerClient))
            {
                KillScreenUI.Instance.SetSmashUI(playerName.ToString());
            }
        }

        public void OnPlayerRespawn()
        {
            enabled = true;
            _isCrash = false;
            _playerHealthController.RestartHealth();
        }

        public void SetShieldActive(bool active) => _isShieldActive = active;
        public void SetSpikeActive(bool active) => _iSpikeActive = active;
    }
}