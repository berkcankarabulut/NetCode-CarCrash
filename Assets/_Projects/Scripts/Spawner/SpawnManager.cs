using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.Spawner
{
    public class SpawnManager : NetworkBehaviour
    {
         [SerializeField] private GameObject _playerPrefab;
         [SerializeField] private List<Transform> _spawnPoints;
         private List<int> _availableSpawnPoints = new List<int>();
         public override void OnNetworkSpawn()
         {
             if(!IsServer) return;
             for (int i = 0; i < _spawnPoints.Count; i++)
             {
                 _availableSpawnPoints.Add(i);
             }

             NetworkManager.OnClientConnectedCallback += SpawnPlayer;
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
             GameObject playerInstance = Instantiate(_playerPrefab, _spawnPoints[spawnIndex].position, _spawnPoints[spawnIndex].rotation);
             playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
         }
    }
}