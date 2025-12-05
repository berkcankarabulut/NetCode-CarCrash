using DG.Tweening;
using UnityEngine;

namespace _Projects.Scripts.UI.GameUIManagement
{
    public class HealthUI : MonoBehaviour
    {
        public static HealthUI Instance { get; private set; }
        [SerializeField] private RectTransform _healthBar;
        [SerializeField] private float _animationDuration;

        private void Awake()
        {
            Instance = this;
        }

        public void SetHealth(int health, int maxHealth)
        {
            _healthBar.DOScaleX(health / maxHealth, _animationDuration).SetEase(Ease.Linear);
        }
    }
}