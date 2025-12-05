using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Projects.Scripts.UI.GameUIManagement
{
    public class KillScreenUI : MonoBehaviour
    {
        public static KillScreenUI Instance { get; private set; }
        public Action onRespawnTimerCompleted;
        [Header("Settings")]
        [SerializeField] private float _scaleDuration = 0.5f;
        [SerializeField] private float _smashUIStayDuration = 1f;
        [Header("Smash UI")] [SerializeField] private RectTransform _smashUIRect;
        [SerializeField] private TMP_Text _smashedPlayerText;

        [Header("Smashed UI")] [SerializeField]
        private RectTransform _smashedUIRect;

        [SerializeField] private TMP_Text _smashedByPlayerText;
        [SerializeField] private TMP_Text _respawnTimerText;

        private float _timer = 0f;
        private bool _isTimerActive = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (!_isTimerActive) return;
            TimeCounter();
        }

        private void TimeCounter()
        {
            _timer -= Time.deltaTime;
            int timer = (int)_timer;
            _respawnTimerText.text = timer.ToString();
            if (_timer > 0) return;
            _smashedPlayerText.text = "";
            _isTimerActive = false;
            _smashedUIRect.gameObject.SetActive(false);
            _smashedUIRect.localScale = Vector3.zero;
            onRespawnTimerCompleted?.Invoke();
        }

        private void Start()
        {
            _smashUIRect.gameObject.SetActive(false);
            _smashedUIRect.gameObject.SetActive(false);
            _smashUIRect.localScale = Vector3.zero;
            _smashedUIRect.localPosition = Vector3.zero;
        }

        public void SetSmashUI(string playerName)
        {
            StartCoroutine(SetSmashUICoroutine(playerName));
        }

        private IEnumerator SetSmashUICoroutine(string playerName)
        {
            _smashUIRect.gameObject.SetActive(true);
            _smashUIRect.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
            _smashedPlayerText.text = playerName;
            yield return new WaitForSeconds(_smashUIStayDuration);
            _smashUIRect.gameObject.SetActive(false);
            _smashUIRect.localScale = Vector3.zero;
            _smashedPlayerText.text = string.Empty;
        }

        public void SetSmashedUI(string playerName, int respawnTimer)
        {
            _smashedUIRect.gameObject.SetActive(true);
            _smashedUIRect.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
            _smashedByPlayerText.text = playerName;
            _respawnTimerText.text = respawnTimer.ToString();
            _isTimerActive = true;
            _timer = respawnTimer;
        }
    }
}