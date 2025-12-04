 
using TMPro;
using UnityEngine;

namespace _Projects.Scripts.UI.GameUIManagement
{
    public class TimerUI : MonoBehaviour
    {
        public static TimerUI Instance { get; private set; }
        [SerializeField] private TMP_Text timerText;

        private void Awake()
        {
            Instance = this;
        }

        public void SetTimerText(int timer)
        {
            timerText.text = timer.ToString();
        }
    }
}