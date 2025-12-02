using System;
using _Projects.Scripts.Helpers;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace _Project.Networking.Client
{
    public class NetworkClient : IDisposable
    {
        private NetworkManager _networkManager;

        public NetworkClient(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            if(clientId != 0 && clientId != _networkManager.LocalClientId) { return; }

            Disconnect();
        }

        public void Disconnect()
        {
            if (SceneManager.GetActiveScene().name != SceneNames.MENU_SCENE)
            {
                SceneManager.LoadScene(SceneNames.MENU_SCENE);
            }

            if (_networkManager.IsConnectedClient)
            {
                _networkManager.Shutdown();
            }
        }

        public void Dispose()
        {
            if (_networkManager == null) { return; }

            _networkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;

            if (_networkManager.IsListening)
            {
                _networkManager.Shutdown();
            }
        }
    }
}