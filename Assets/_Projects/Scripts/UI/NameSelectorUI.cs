using System;
using _Projects.Helpers.Const;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Projects.Scripts.UI.NameSelect
{
    public class NameSelectorUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private Button _connectButton;

        private void OnEnable()
        {
            _connectButton.onClick.AddListener(OnClickedConnect);
        }

        private void OnDisable()
        {
            _connectButton.onClick.RemoveAllListeners();
        }

        private void Awake()
        {
            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
            {
                SceneManager.LoadScene(SceneNames.LOADING_SCENE);
                return;
            }
            
            _nameInputField.text = PlayerPrefs.GetString(PlayerData.PLAYER_NAME, string.Empty);
        }

        private void OnClickedConnect()
        {
            _connectButton.interactable = false;
            PlayerPrefs.SetString(PlayerData.PLAYER_NAME, _nameInputField.text);
            SceneManager.LoadScene(SceneNames.LOADING_SCENE);
        }
    }
}