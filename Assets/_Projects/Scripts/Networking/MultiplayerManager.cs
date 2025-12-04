using System;
using _Projects.Scripts.Serializables;
using Unity.Netcode;

namespace _Projects.Networking
{
    public class MultiplayerManager : NetworkBehaviour
    {
        public static MultiplayerManager Instance { private set; get; }
        private NetworkList<PlayerDataSerializable> _playerDataNetworkList = new NetworkList<PlayerDataSerializable>();
        public event Action OnPlayerDataNetworkListChanged;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _playerDataNetworkList.OnListChanged += OnListChanged;
        }

        private void OnDisable()
        {
            _playerDataNetworkList.OnListChanged -= OnListChanged;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            _playerDataNetworkList.Clear();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void OnClientConnected(ulong clientId)
        {
            for (int i = 0; i < _playerDataNetworkList.Count; i++)
            {
                if (!_playerDataNetworkList[i].Equals(clientId)) continue;
                _playerDataNetworkList.RemoveAt(i);
            }

            _playerDataNetworkList.Add(new PlayerDataSerializable
            {
                ClientId = clientId
            });
        }

        private void OnClientDisconnected(ulong clientId)
        {
              for (int i = 0; i < _playerDataNetworkList.Count; i++)
              {
                  if (!_playerDataNetworkList[i].Equals(clientId)) continue;
                  _playerDataNetworkList.RemoveAt(i);
              }
        }
        

        private void OnListChanged(NetworkListEvent<PlayerDataSerializable> changeevent)
        {
            OnPlayerDataNetworkListChanged?.Invoke();
        }

        public bool IsPlayerIndexConnected(int playerIndex)
        {
            return playerIndex < _playerDataNetworkList.Count;
        }

        public PlayerDataSerializable GetPlayerData(int playerIndex)
        {
            return _playerDataNetworkList[playerIndex];
        }
    }
}