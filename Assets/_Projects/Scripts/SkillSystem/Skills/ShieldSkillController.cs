using Game.Player;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.Scripts.SkillSystem
{
    public class ShieldSkillController : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            PlayerSkillController.onTimerCompleted += OnSkillCompleted;
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

        public override void OnNetworkDespawn()
        {
            PlayerSkillController.onTimerCompleted -= OnSkillCompleted;
        }
    }
}