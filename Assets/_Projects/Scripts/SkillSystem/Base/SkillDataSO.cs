using UnityEngine;

[CreateAssetMenu(fileName = "SkillDataSO", menuName = "Scriptable Objects/SkillDataSO")]
public class SkillDataSO : ScriptableObject
{
    [SerializeField] private Transform _skillPrefab;
    public Transform SkillPrefab => _skillPrefab;
}
     