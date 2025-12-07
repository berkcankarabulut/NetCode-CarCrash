using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.Scripts.UI.GameUIManagement
{
    public class LeaderBoardRanking : MonoBehaviour
    {
        [SerializeField] private TMP_Text _rankText;
        [SerializeField] private TMP_Text _playerNameText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private Color _ownerColor;

        public ulong ClientId { get; private set; }
        private FixedString32Bytes _playerName;
        public FixedString32Bytes LeaderBoardEntites { get; private set; }
        public int Score { get; private set; }
        public void SetData(ulong clientId, FixedString32Bytes playerName, int score)
        {
            ClientId = clientId;
            _playerName = playerName;

            _playerNameText.text = _playerName.ToString();
        
            if(clientId == NetworkManager.Singleton.LocalClientId)
            {
                _playerNameText.color = _ownerColor;
                _scoreText.color = _ownerColor;
            }

            UpdateOrder();
            UpdateScore(score);
        }

        public void UpdateScore(int score)
        {
            Score = score;
            _scoreText.text = Score.ToString();
        }

        public void UpdateOrder()
        {
            _rankText.text = $"{transform.GetSiblingIndex() + 1}";
        }

        public string GetPlayerName()
        {
            return _playerName.ToString();
        }
    }
}