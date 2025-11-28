using UnityEngine;

namespace _Projects.Scripts.SkillSystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/MysteryBoxSkillSO", fileName = "MysteryBoxSkillSO")]
    public class MysteryBoxSkillSO : ScriptableObject
    {
        [Header("Visual Information")]
        [SerializeField] private string _skillName;
        [SerializeField] private Sprite _icon;
        
        [Header("Skill Usage")]
        [SerializeField] private SkillDataSO _skillData;
        [SerializeField] private SkillTypes _skillType;
        [SerializeField] private SkillUsageType _skillUsageType;
        
        public string SkillName => _skillName;
        public Sprite Icon => _icon;
        public SkillDataSO SkillData => _skillData;
        public SkillTypes SkillType => _skillType;
        public SkillUsageType SkillUsageType => _skillUsageType;
    }
}