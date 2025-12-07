using _Projects.Player;
using _Projects.Scripts.UI.GameUIManagement;
using _Projects.SkillSystem;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.Damageables
{
    public class FakeBoxDamageable : NetworkBehaviour, IDamageable
    {
        [SerializeField] private MysteryBoxSkillSO _skill;
        private PlayerVehicleController vehicleController;

        public int GetRespawnTimer => _skill.SkillData.RespawnTimer;
        public int GetDamageAmount => _skill.SkillData.DamageAmount;

        public string GetKillerName()
        {
            ulong killerClientId = GetKillerClientID();
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientId, out var killerClient))
            {
                string playerName = killerClient.PlayerObject.GetComponent<PlayerNetworkController>().PlayerName.Value
                    .ToString();
                return playerName;
            }

            return string.Empty;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var client))
            {
                NetworkObject owner = client.PlayerObject;
                vehicleController = owner.GetComponent<PlayerVehicleController>();
                vehicleController.OnVehicleCrashed += VehicleCrash;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (vehicleController == null) return;
            vehicleController.OnVehicleCrashed -= VehicleCrash;
        }

        private void VehicleCrash()
        {
            DestroyRPC();
        }

        public void Damage(PlayerVehicleController vehicle, string playerName)
        {
            vehicle.CrashVehicle();
            KillScreenUI.Instance.SetSmashedUI(playerName, _skill.SkillData.RespawnTimer);
            DestroyRPC();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ShieldSkillController shieldSkillController))
            {
                DestroyRPC();
            }
        }


        public ulong GetKillerClientID()
        {
            return OwnerClientId;
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void DestroyRPC()
        {
            if (IsServer)
            {
                Destroy(gameObject);
            }
        }
    }
}