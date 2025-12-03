using _Project.Networking.Client;
using _Project.Networking.Host;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;
        [SerializeField] private TMP_InputField _hostInputField;
        private void OnEnable()
        {
            _hostButton.onClick.AddListener(StartHost);
            _clientButton.onClick.AddListener(StartClient);
        }

        private void OnDisable()
        {
            _hostButton.onClick.RemoveListener(StartHost);
            _clientButton.onClick.RemoveListener(StartClient);
        }
        private async void StartHost()
        {
            _hostButton.interactable = false;
            await HostSingleton.Instance.HostManager.StartHostAsync();
        }

        private async void StartClient()
        {
            await ClientSingleton.Instance.ClientManager.StartClientAsync(_hostInputField.text);
        }
    }
}