using UnityEngine;

[CreateAssetMenu(fileName = "SkillDataSO", menuName = "Scriptable Objects/SkillDataSO")]
public class SkillDataSO : ScriptableObject
{
    [SerializeField] private Transform _skillPrefab;
    [SerializeField] private Vector3 _skillOffset;
    [SerializeField] private int _spawnAmountOrTimer;
    [SerializeField] private bool _shouldBeAttachedToParent;
    [SerializeField] private int _respawnTimer = 5;
    [SerializeField] private int _damageAmount = 100;
    public Transform SkillPrefab => _skillPrefab;
    public Vector3 SkillOffset => _skillOffset;
    public int SpawnAmountOrTimer => _spawnAmountOrTimer;
    public bool ShouldBeAttachedToParent => _shouldBeAttachedToParent;
    public int RespawnTimer => _respawnTimer;
    public int DamageAmount => _damageAmount;
}
     