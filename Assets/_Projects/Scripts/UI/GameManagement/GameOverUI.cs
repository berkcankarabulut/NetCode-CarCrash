using System;
using _Projects.GameManagement;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Projects.UI.GameUIManagement
{
    public class GameOverUI : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Image _gameOverBackgroundImage;

        [SerializeField] private RectTransform _gameOverTransform;
        [SerializeField] private RectTransform _scoreTableTransform;
        [SerializeField] private TMP_Text _winnerText;
        [SerializeField] private Button _mainMenuButton;
        private RectTransform _mainMenuTransform;
        private RectTransform _winnerTransform;
        private float _animationDuration = .6f;

        private void Awake()
        {
            _mainMenuTransform = _mainMenuButton.GetComponent<RectTransform>();
            _winnerTransform = _winnerText.GetComponent<RectTransform>();
        }

        private void Start()
        {
            _scoreTableTransform.gameObject.SetActive(false);
            _scoreTableTransform.localScale = Vector3.zero;
            GameManager.Instance.OnGameStateChanged += GameStateChanged;
        }

        private void GameStateChanged(GameState gameState)
        {
            if (gameState != GameState.GameOver) return;
            AnimateGameOver();
            GameManager.Instance.OnGameStateChanged -= GameStateChanged;
        }

        private void AnimateGameOver()
        {
            _gameOverBackgroundImage.DOFade(.8f, _animationDuration / 2);
            _gameOverTransform.DOAnchorPosY(0f, _animationDuration).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                _gameOverTransform.GetComponent<TMP_Text>().DOFade(0f, _animationDuration / 2).SetDelay(.2f).OnComplete(() =>
                {
                    AnimateLeaderBoardAndButtons();
                });;
            });
        }

        private void AnimateLeaderBoardAndButtons()
        {
          
            _scoreTableTransform.gameObject.SetActive(true);
            _scoreTableTransform.DOScale(Vector3.one, 1).SetEase(Ease.OutBack).OnComplete(() =>
            {
                _winnerTransform.DOScale(1f, _animationDuration).OnComplete(() =>
                {
                    _mainMenuTransform.DOScale(1f, _animationDuration).SetDelay(.2f).SetEase(Ease.OutBack);
                });
            });
        }
    }
}