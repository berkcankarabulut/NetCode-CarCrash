using Unity.Netcode;
using UnityEngine;

namespace _Project.Collect
{
    public class MysteryBoxCollectible : NetworkBehaviour, ICollectible
    {
        private static readonly int IsCollected = Animator.StringToHash("IsCollected");
        private static readonly int IsRespawned = Animator.StringToHash("IsRespawned");
        
        [Header("Settings")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Collider _collider;
        [SerializeField] private float _respawnTime = 5;

        public void Collect()
        {
            CollectRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void CollectRpc()
        { 
            AnimateCollection();
            Invoke(nameof(Respawn), _respawnTime);
        }

        private void AnimateCollection()
        {
            _collider.enabled = false;
            _animator.SetTrigger(IsCollected);
        }

        private void Respawn()
        {
            _animator.SetTrigger(IsRespawned);
            _collider.enabled = true;
        }
    }
}