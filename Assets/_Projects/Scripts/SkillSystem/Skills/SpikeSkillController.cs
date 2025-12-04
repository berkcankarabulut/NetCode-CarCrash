using _Projects.Player;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.SkillSystem
{
    public class SpikeSkillController : NetworkBehaviour
    {
        [SerializeField] private Collider _collider;
        public override void OnNetworkSpawn()
        {
            PlayerSkillController.onTimerCompleted += OnSkillCompleted;
            if (IsOwner) SetOwnerVisualsRPC();
        }

        private void OnSkillCompleted()
        {
            DestroyRPC();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void DestroyRPC()
        {
            if (IsServer)
            {
                Destroy(gameObject);
            }
        }

        [Rpc(SendTo.Owner)]
        private void SetOwnerVisualsRPC()
        {
            _collider.enabled = false;
        }

        public override void OnNetworkDespawn()
        {
            PlayerSkillController.onTimerCompleted -= OnSkillCompleted;
        }
    }
}