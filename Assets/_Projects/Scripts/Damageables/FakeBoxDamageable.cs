using System;
using _Projects.Scripts.SkillSystem;
using Game.Player;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.Scripts.Damageables
{
    public class FakeBoxDamageable : NetworkBehaviour, IDamageable
    {
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
            vehicleController.OnVehicleCrashed -= VehicleCrash;
        }

        private void VehicleCrash()
        {
            DestroyRPC();
        }

        public void Damage(PlayerVehicleController vehicle)
        {
            vehicle.CrashVehicle();
            DestroyRPC();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ShieldSkillController shieldSkillController))
            {
                DestroyRPC();
            }
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