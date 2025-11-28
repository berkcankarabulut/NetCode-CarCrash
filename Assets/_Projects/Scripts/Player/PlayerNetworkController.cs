using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Game.Player
{
    public class PlayerNetworkController : NetworkBehaviour
    {
        [SerializeField] private CinemachineCamera _playerCamera; 
        public override void OnNetworkSpawn()
        {
            _playerCamera.gameObject.SetActive(IsOwner);
        }
    }
}