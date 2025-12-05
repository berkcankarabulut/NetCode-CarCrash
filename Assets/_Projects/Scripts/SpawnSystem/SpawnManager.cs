using System.Collections;
using System.Collections.Generic;
using _Projects.GameManagement;
using _Projects.Player;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Projects.SpawnSystem
{
    public class SpawnManager : NetworkBehaviour
    {
        public static SpawnManager Instance { get; private set; }
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private List<Transform> _spawnPoints;

        private List<int> _availableRespawnPoints = new List<int>();
        private List<int> _availableSpawnPoints = new List<int>();

        private void Awake()
        {
            Instance = this;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            for (int i = 0; i < _spawnPoints.Count; i++)
            {
                _availableSpawnPoints.Add(i);
            }

            for (int i = 0; i < _availableRespawnPoints.Count; i++)
            {
                _availableRespawnPoints.Add(i);
            }

            SpawnAllPlayers();
        }

        private void SpawnAllPlayers()
        {
            if (!IsServer) return;
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                SpawnPlayer(client.ClientId);
            }
        }
        
        private void SpawnPlayer(ulong clientId)
        {
            if (_availableSpawnPoints.Count == 0)
            {
                Debug.LogWarning("No available players spawned.");
                return;
            }

            int randomIndex = Random.Range(0, _availableSpawnPoints.Count);
            int spawnIndex = _availableSpawnPoints[randomIndex];
            _availableSpawnPoints.RemoveAt(randomIndex);
            GameObject playerInstance = Instantiate(_playerPrefab, _spawnPoints[spawnIndex].position,
                _spawnPoints[spawnIndex].rotation);
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }

        public void RespawnPlayer(int respawnTimer, ulong clientId)
        {
            StartCoroutine(RespawnPlayerCoroutine(respawnTimer, clientId));
        }

        private IEnumerator RespawnPlayerCoroutine(int respawnTimer, ulong clientId)
        {
            yield return new WaitForSeconds(respawnTimer); 
            if (GameManager.Instance.GameState != GameState.Playing) yield break;
            if (_spawnPoints.Count == 0)
            {
                Debug.LogWarning("No available respawn points.");
                yield break;
            }

            if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
            {
                Debug.LogWarning("No client with id " + clientId + " was found.");
                yield break;
            }

            if (_availableRespawnPoints.Count == 0)
            {
                for (int i = 0; i < _spawnPoints.Count; i++)
                {
                    _availableRespawnPoints.Add(i);
                }
            }

            int randomIndex = Random.Range(0, _availableRespawnPoints.Count);
            int spawnIndex = _availableRespawnPoints[randomIndex];
            _availableRespawnPoints.RemoveAt(randomIndex);

            Transform respawnPoint = _spawnPoints[spawnIndex];
            NetworkObject player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            if (player == null)
            {
                Debug.LogWarning("No player with id " + clientId + " was found.");
                yield break;
            }

            if (player.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.isKinematic = true;
            }

            if (player.TryGetComponent<NetworkTransform>(out var networkTransform))
            {
                networkTransform.Interpolate = false;
                networkTransform.GetComponent<PlayerVehicleVisual>().SetVehicleVisualActive(0.1f);
            }

            player.transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
            yield return new WaitForSeconds(.1f);
            rigidbody.isKinematic = false;
            networkTransform.Interpolate = true;
            if (player.TryGetComponent<PlayerNetworkController>(out var playerNetworkController))
            {
                playerNetworkController.OnPlayerRespawn();
            }
        }
    }
}