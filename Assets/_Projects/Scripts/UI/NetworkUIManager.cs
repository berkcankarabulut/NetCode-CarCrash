using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI.Network
{
    public class NetworkUIManager : MonoBehaviour
    {
        [Header("Panel")] [SerializeField] private GameObject _networkPanel;

        [Header("Buttons")] 
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;

        private void OnEnable()
        {
            _hostButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
                Close();
            });
            _clientButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
                Close();
            });
        }

        private void OnDisable()
        {
            _hostButton?.onClick.RemoveListener(() => { NetworkManager.Singleton.StartHost(); });
            _clientButton?.onClick.RemoveListener((() => NetworkManager.Singleton.StartClient()));
        }

        private void Close()
        {
            _networkPanel.SetActive(false);
        }
    }
}