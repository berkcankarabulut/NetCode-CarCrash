using System;
using _Projects.CharacterSelect;
using _Projects.Helpers.Const;
using _Projects.Networking;
using _Projects.Networking.Client;
using _Projects.Networking.Host;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Projects.Scripts.UI
{
    public class CharacterSelectUI : MonoBehaviour
    {
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _copyButton;
        [SerializeField] private TMP_Text _readyText;

        [Header("Ready UIs")] 
        [SerializeField] private Sprite _readySprite;
        [SerializeField] private Sprite _unReadySprite;

        private bool _isPlayerReady;

        private void Awake()
        {
            _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            _readyButton.onClick.AddListener(OnReadyButonClicked);
            _startButton.onClick.AddListener(OnStartButtonClicked);
        }

        private void Start()
        {
            _startButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

            CharacterSelectReady.Instance.OnAllReadyChanged += OnAllPlayersReady;
            CharacterSelectReady.Instance.OnUnReadyChanged += OnPlayersUnReady;
            MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += OnPlayerDataNetworkListChanged;
        }

        private void OnAllPlayersReady()
        {
            SetStartButtonInteractable(true);
        }

        private void OnPlayersUnReady()
        {
            SetStartButtonInteractable(false);
        }

        private void OnPlayerDataNetworkListChanged()
        {
            SetStartButtonInteractable(CharacterSelectReady.Instance.AreAllPlayersReady());
        }

        private void OnMainMenuButtonClicked()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                HostSingleton.Instance.HostManager.ShutDown();
            }

            HostSingleton.Instance.HostManager.RemovePlayerFromLobby();
            ClientSingleton.Instance.ClientManager.Disconnect(); 
        }

        private void OnReadyButonClicked()
        {
            _isPlayerReady = !_isPlayerReady;
            if (_isPlayerReady)
            {
                SetPlayerReady();
            }
            else
            {
                SetPlayerUnReady();
            }
        }

        private void OnStartButtonClicked()
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneNames.GAME_SCENE, LoadSceneMode.Single);
        }

        private void SetPlayerReady()
        {
            CharacterSelectReady.Instance.SetPlayerReady();
            _readyText.text = "Ready";
            _readyButton.image.sprite = _readySprite;
        }

        private void SetPlayerUnReady()
        {
            CharacterSelectReady.Instance.SetPlayerUnready();
            _readyText.text = "Not Ready";
            _readyButton.image.sprite = _unReadySprite;
        }

        private void SetStartButtonInteractable(bool isInteractable)
        {
            if (_startButton == null) return;
            _startButton.interactable = isInteractable;
        }
    }
}