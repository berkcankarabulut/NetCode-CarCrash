using System;
using _Projects.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace _Projects.Scripts.UI.CharacterSelect
{
    public class CharacterColorSelectSingleUI : MonoBehaviour
    {
        [SerializeField] private int _colorId;
        [SerializeField] private Image _colorImage;
        [SerializeField] private GameObject _selectedGameObject;
        [SerializeField] private Button _selectButton;

        private void Awake()
        {
            _selectButton.onClick.AddListener(() =>
            {
                if (CharacterSelectUI.Instance.IsPlayerReady()) return;
                MultiplayerManager.Instance.ChangePlayerColor(_colorId);
            });
        }

        private void Start()
        {
            MultiplayerManager.Instance.OnPlayerDataNetworkListChanged +=
                OnPlayerDataNetworkListChanged;

            _colorImage.color = MultiplayerManager.Instance.GetPlayerColor(_colorId);
            UpdateIsSelected();
        }

        private void OnPlayerDataNetworkListChanged()
        {
            UpdateIsSelected();
        }

        private void UpdateIsSelected()
        {
            _selectedGameObject.SetActive(MultiplayerManager.Instance.GetPlayerData().ColorId == _colorId);
        }

        private void OnDestroy()
        {
            MultiplayerManager.Instance.OnPlayerDataNetworkListChanged -=
                OnPlayerDataNetworkListChanged;
        }
    }
}