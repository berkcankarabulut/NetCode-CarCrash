using System;
using _Projects.UI.GameUIManagement;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.GameManagement
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }
        public Action<GameState> OnGameStateChanged;
        
        [SerializeField] private GameDataSO _gameDataSO;
        [SerializeField] private GameState _gameState;
        private NetworkVariable<int> _gameTimer = new NetworkVariable<int>(0);

        public GameState GameState => _gameState;

        private void Awake()
        {
            Instance = this;
        } 
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _gameTimer.Value = _gameDataSO.GameTimer;
                SetTimerTextRPC();
                InvokeRepeating(nameof(DecreaseTimerText), 1f, 1f);
            }

            _gameTimer.OnValueChanged += OnTimerChanged;
        }

        private void OnTimerChanged(int previousvalue, int newvalue)
        {
            TimerUI.Instance.SetTimerText(newvalue);
            if (!IsServer || newvalue > 0) return;
            ChangeGameStateRPC(GameState.GameOver);
            _gameTimer.OnValueChanged -= OnTimerChanged;
        }

        public void ChangeGameState(GameState gameState)
        {
            if(!IsServer) return;
            _gameState = gameState;
            ChangeGameStateRPC(gameState);
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void ChangeGameStateRPC(GameState gameState)
        {
            _gameState = gameState;
            OnGameStateChanged?.Invoke(gameState);
            Debug.Log($"Game state changed to {this.GameState}");
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SetTimerTextRPC()
        {
            TimerUI.Instance.SetTimerText(_gameTimer.Value);
        }

        private void DecreaseTimerText()
        {
            if (!IsServer || GameState != GameState.Playing) return;
            _gameTimer.Value--;
            if (_gameTimer.Value <= 0)
                CancelInvoke(nameof(DecreaseTimerText));
        }
    }
}