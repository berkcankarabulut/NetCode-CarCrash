using _Project.Collect;
using Unity.Netcode;
using UnityEngine;

namespace Game.Player
{
    public class PlayerInteractionController : NetworkBehaviour
    {
        PlayerSkillController _playerSkillController;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsOwner) return;
            _playerSkillController = GetComponent<PlayerSkillController>();
        }

        private void OnTriggerEnter(Collider other)
        { 
            if(!IsOwner) return;
            if (_playerSkillController.HasSkillAlready) return; 
            
            ICollectible collectible = other.gameObject.GetComponent<ICollectible>();
            if (collectible == null) return;
            collectible.Collect(_playerSkillController);
        }
    }
}