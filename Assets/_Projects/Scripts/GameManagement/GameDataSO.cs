using UnityEngine;

namespace _Projects.GameManagement
{
    [CreateAssetMenu(fileName = "New Game Data", menuName = "Scriptable Objects/GameDataSO")]
    public class GameDataSO : ScriptableObject
    {
        [SerializeField] private int _gameTimer;
        public int GameTimer => _gameTimer;
    }
}