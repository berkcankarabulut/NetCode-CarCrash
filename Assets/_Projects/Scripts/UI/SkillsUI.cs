using System;
using _Projects.Scripts.SkillSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI.SkillSystem
{
    public class SkillsUI : MonoBehaviour
    {
        public static SkillsUI Instance { get; private set; }

        [Header("Skill References")] [SerializeField]
        private Image _skillImage;

        [SerializeField] private TMP_Text _skillName;
        [SerializeField] private TMP_Text _timerCounter;
        [SerializeField] private Transform _timerCounterParentTransform;

        Tween _timerCounterTween;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetSkillToNull();

            _timerCounterParentTransform.localScale = Vector3.zero;
            _timerCounterParentTransform.gameObject.SetActive(false);
        }

        public void SetSkill(string skillName, Sprite skillSprite, SkillUsageType skillUsageType, int timerCounter)
        {
            _skillImage.gameObject.SetActive(true);
            _skillImage.sprite = skillSprite;
            _skillName.text = skillName;
            if (skillUsageType != SkillUsageType.None)
            {
                SetTimerCounterAnimation(timerCounter);
            }
        }

        private void SetTimerCounterAnimation(int timerCounter)
        { 
            _timerCounterParentTransform.gameObject.SetActive(true);
            _timerCounterTween = _timerCounterParentTransform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
            _timerCounter.text = timerCounter.ToString();
        }

        public void SetSkillToNull()
        { 
            _skillImage.gameObject.SetActive(false);
            _skillImage.sprite = null;
            _skillName.text = "";
             
            _timerCounterTween?.Kill();
            _timerCounterTween = _timerCounterParentTransform.DOScale(Vector3.zero, 0).SetEase(Ease.OutBack);
        }

        public void SetTimerOrAmountCounterText(int timerCounter)
        {
            _timerCounter.text = timerCounter.ToString();
        }
    }
}