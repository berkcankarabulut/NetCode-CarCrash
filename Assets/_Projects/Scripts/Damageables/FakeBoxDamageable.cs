using System;
using _Project.UI.Network;
using _Projects.Scripts.SkillSystem;
using Game.Player;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.Scripts.Damageables
{
    public class FakeBoxDamageable : NetworkBehaviour, IDamageable
    {
        [SerializeField] private MysteryBoxSkillSO _skill;
        private PlayerVehicleController vehicleController;
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
            if(vehicleController == null) return;
            vehicleController.OnVehicleCrashed -= VehicleCrash;
        }

        private void VehicleCrash()
        {
            DestroyRPC();
        }

        public void Damage(PlayerVehicleController vehicle)
        {
            vehicle.CrashVehicle();
            KillScreenUI.Instance.SetSmashedUI("Kaju", _skill.SkillData.RespawnTimer);
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