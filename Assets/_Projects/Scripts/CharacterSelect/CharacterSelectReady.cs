using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace _Projects.CharacterSelect
{
    public class CharacterSelectReady : NetworkBehaviour
    {
        public static CharacterSelectReady Instance { private set; get; }
        public event Action OnReadyChanged;
        public event Action OnUnReadyChanged;
        public event Action OnAllReadyChanged;

        private Dictionary<ulong, bool> _playerReady;

        private void Awake()
        {
            Instance = this;
            _playerReady = new Dictionary<ulong, bool>();
        }

        public override void OnDestroy()
        {
            Instance = null;
        }

        public override void OnNetworkSpawn()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallBack;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallBack;
        }

        private void OnClientConnectedCallBack(ulong connectedClientId)
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!IsPlayerReady(clientId)) continue;
                SetPlayerReadyToAllRpc(clientId);
            }
        }

        private void OnClientDisconnectCallBack(ulong clientId)
        {
            if (!_playerReady.ContainsKey(clientId)) return;
            _playerReady.Remove(clientId);
            OnUnReadyChanged?.Invoke();
        }

        [Rpc(SendTo.Server)]
        private void SetPlayerReadyRpc(RpcParams rpcParams = default)
        {
            SetPlayerReadyToAllRpc(rpcParams.Receive.SenderClientId);
            _playerReady[rpcParams.Receive.SenderClientId] = true;
            bool allClientsReady = true;

            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (_playerReady.ContainsKey(clientId) && _playerReady[clientId]) continue;
                allClientsReady = false;
                break;
            }

            if (allClientsReady)
            {
                OnAllReadyChanged?.Invoke();
            }
        }

        [Rpc(SendTo.Server)]
        private void SetPlayerUnReadyRpc(RpcParams rpcParams = default)
        {
            SetPlayerUnReadyToAllRpc(rpcParams.Receive.SenderClientId);
            if (_playerReady.ContainsKey(rpcParams.Receive.SenderClientId))
            {
                _playerReady[rpcParams.Receive.SenderClientId] = false;
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SetPlayerReadyToAllRpc(ulong clientId)
        {
            _playerReady[clientId] = true;
            OnReadyChanged?.Invoke();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SetPlayerUnReadyToAllRpc(ulong clientId)
        {
            _playerReady[clientId] = false;
            OnReadyChanged?.Invoke();
            OnUnReadyChanged?.Invoke();
        }

        public bool IsPlayerReady(ulong playerId)
        {
            return _playerReady.ContainsKey(playerId) && _playerReady[playerId];
        }

        public bool AreAllPlayersReady()
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (_playerReady.ContainsKey(clientId) && _playerReady[clientId]) continue;
                return false;
            } 
            return true;
        }

        public void SetPlayerReady()
        {
            SetPlayerReadyRpc();
        }

        public void SetPlayerUnready()
        {
            SetPlayerUnReadyRpc();
        }
    }
}