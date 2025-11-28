using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI.SkillSystem
{
    public class SkillsUI : MonoBehaviour
    {
        public static SkillsUI Instance { get; private set; }
        [Header("Skill References")]
        [SerializeField] private Image _skillImage;
        [SerializeField] private TMP_Text _skillName;
        [SerializeField] private TMP_Text _timerCounter;
        [SerializeField] private Transform _timerCounterParentTransform;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetSkillToNull();
        }

        public void SetSkill(string skillName, Sprite skillSprite)
        {
            _skillImage.gameObject.SetActive(true);
            _skillImage.sprite = skillSprite;
            _skillName.text = skillName;
        }

        public void SetSkillToNull()
        {
            _skillImage.gameObject.SetActive(false);
            _skillImage.sprite = null;
            _skillName.text = ""; 
        }
    }
}