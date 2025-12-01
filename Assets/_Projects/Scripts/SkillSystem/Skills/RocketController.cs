using Unity.Netcode;
using UnityEngine;

namespace _Projects.Scripts.SkillSystem
{
    public class RocketController : NetworkBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private float _moveSpeed = 1;
        [SerializeField] private float _rotationSpeed = 1;
        private bool _isMoving = false;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            SetOwnerVisualRPC();
            RequestStartMoveFromServerRPC();
        }

        [Rpc(SendTo.Owner)]
        private void SetOwnerVisualRPC()
        {
            _collider.enabled = false;
        }

        private void Update()
        {
            if (!IsServer && !_isMoving) return;
            MoveRocket();
        }

        private void MoveRocket()
        {
            transform.position += _moveSpeed * transform.forward * Time.deltaTime;
            transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime, Space.Self);
        }

        [Rpc(SendTo.Server)]
        private void RequestStartMoveFromServerRPC()
        {
            _isMoving = true;
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