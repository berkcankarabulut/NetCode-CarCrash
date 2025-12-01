using System;
using System.Collections;
using _Project.SkillSystem;
using _Project.UI.SkillSystem;
using _Projects.Scripts.SkillSystem;
using Unity.Netcode;
using UnityEngine;

namespace Game.Player
{
    public class PlayerSkillController : NetworkBehaviour
    {
        public static Action onTimerCompleted;

        [SerializeField] private Transform _rocketLauncherTransform;
        [SerializeField] private Transform _rocketLaunchPoint;
        [SerializeField] private bool _hasSkillAlready;
        private PlayerVehicleController _playerVehicleController;
        private PlayerInteractionController _playerInteractionController;
        private MysteryBoxSkillSO _skill;
        private bool _isSkillused;
        private bool _hasTimerStarted;
        private float _timer;
        private float _timerMax;
        private float _resetDelayRocketLauncherClose = .5f;
        private int _mineAmountCounter;
        public bool HasSkillAlready => _hasSkillAlready;

        private void Update()
        {
            if (!IsOwner) return;
            if (Input.GetKeyDown(KeyCode.Space) && !_isSkillused)
            {
                ActivateSkill();
                _isSkillused = true;
            }

            TimeCounter();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            _playerVehicleController = GetComponent<PlayerVehicleController>();
            _playerInteractionController = GetComponent<PlayerInteractionController>();
            _playerVehicleController.OnVehicleCrashed += OnVehicleCrashed;
        }

        private void OnVehicleCrashed()
        {
            enabled = false;
            SkillsUI.Instance.SetSkillToNull();
            _hasSkillAlready = true;
            _isSkillused = false;
            SetRocketLauncherActiveRPC(false);
            _playerVehicleController.OnVehicleCrashed -= OnVehicleCrashed;
        }

        private void TimeCounter()
        {
            if (!_hasTimerStarted) return;
            _timer -= Time.deltaTime;
            SkillsUI.Instance.SetTimerOrAmountCounterText((int)_timer);
            if (_timer <= 0)
            {
                SkillsUI.Instance.SetSkillToNull();
                _hasTimerStarted = false;
                _hasSkillAlready = false;
                onTimerCompleted?.Invoke();
                if (_skill.SkillType == SkillTypes.Shield)
                {
                    _playerInteractionController.SetShieldActive(false);
                }
                else if (_skill.SkillType == SkillTypes.Spike)
                {
                    _playerInteractionController.SetShieldActive(false);
                }
            }
        }

        public void SetupSkill(MysteryBoxSkillSO skill)
        {
            _hasSkillAlready = true;
            _skill = skill;
            _isSkillused = false;

            if (skill.SkillType == SkillTypes.Rocket)
            {
                SetRocketLauncherActiveRPC(true);
            }
        }

        private IEnumerator ResetRocketLauncher()
        {
            yield return new WaitForSeconds(_resetDelayRocketLauncherClose);
            SetRocketLauncherActiveRPC(false);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SetRocketLauncherActiveRPC(bool active)
        {
            _rocketLauncherTransform.gameObject.SetActive(active);
        }

        private void SetToAmountOrTimer()
        {
            switch (_skill.SkillUsageType)
            {
                case SkillUsageType.None:
                    SkillsUI.Instance.SetSkillToNull();
                    _hasSkillAlready = false;
                    break;
                case SkillUsageType.Timer:
                    _hasTimerStarted = true;
                    _timerMax = _skill.SkillData.SpawnAmountOrTimer;
                    _timer = _timerMax;
                    break;
                case SkillUsageType.Amount:
                    _mineAmountCounter = (int)_skill.SkillData.SpawnAmountOrTimer;
                    SkillManager.Instance.OnMineCountReduced += OnMineReduced;
                    break;
            }
        }

        private void OnMineReduced()
        {
            _mineAmountCounter--;
            SkillsUI.Instance.SetTimerOrAmountCounterText(_mineAmountCounter);
            if (_mineAmountCounter > 0) return;
            _hasSkillAlready = false;
            SkillsUI.Instance.SetSkillToNull();
            SkillManager.Instance.OnMineCountReduced -= OnMineReduced;
        }

        private void ActivateSkill()
        {
            if (!HasSkillAlready) return;
            SkillManager.Instance.ActivateSkill(_skill.SkillType, transform, OwnerClientId);
            SetToAmountOrTimer();

            if (_skill.SkillType == SkillTypes.Rocket)
                StartCoroutine(ResetRocketLauncher());
            else if (_skill.SkillType == SkillTypes.Shield)
            {
                _playerInteractionController.SetShieldActive(true);
            }
            else if (_skill.SkillType == SkillTypes.Spike)
            {
                _playerInteractionController.SetSpikeActive(true);
            }
        }

        public Vector3 GetRocketLauncherPosition()
        {
            return _rocketLaunchPoint.position;
        }
    }
}