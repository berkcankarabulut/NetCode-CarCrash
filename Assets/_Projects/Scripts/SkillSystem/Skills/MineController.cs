using System;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.Scripts.SkillSystem
{
    public class MineController : NetworkBehaviour
    {
        [SerializeField] private Collider _collider;

        [Header("Settings")] 
        [SerializeField] private float _fallSpeed;
        [SerializeField] private float _raycastDistance;
        [SerializeField] private LayerMask _groundMask;
        private bool _hasLanded;
        private Vector3 _lastSentPosition;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                SetOwnerVisualRPC();
            }
        }

        private void Update()
        {
            if (!IsServer|| _hasLanded) return;
            SetPositionOnGround();
        }

        private void SetPositionOnGround()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _raycastDistance, _groundMask))
            {
                _hasLanded = true;
                transform.position = hit.point;
                SyncPosition();
            }
            else
            {
                transform.position += Vector3.down * (_fallSpeed * Time.deltaTime);
                SyncPosition();
            }
        }

        private void SyncPosition()
        {
            if (_lastSentPosition != transform.position)
            {
                SetSyncPositionRPC(transform.position);
                _lastSentPosition = transform.position;
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SetSyncPositionRPC(Vector3 newPosition)
        {
            if(IsServer) return;
            transform.position = newPosition;
        }
        [Rpc(SendTo.Owner)]
        private void SetOwnerVisualRPC()
        {
            _collider.enabled = false;
        }
    }
}