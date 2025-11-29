using System;
using _Project.Collect;
using _Project.SkillSystem;
using _Project.UI.SkillSystem;
using _Projects.Scripts.SkillSystem;
using Unity.Netcode;
using UnityEngine;

namespace Game.Player
{
    public class PlayerSkillController : NetworkBehaviour
    {
        [SerializeField] private bool _hasSkillAlready;
        private MysteryBoxSkillSO _skill;
        private bool _isSkillused;

        public bool HasSkillAlready => _hasSkillAlready;

        private void Update()
        {
            if (!IsOwner) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ActivateSkill();
                _isSkillused = true;
            }
        }

        public void SetupSkill(MysteryBoxSkillSO skill)
        {
            _hasSkillAlready = true;
            _skill = skill;
            _isSkillused = false;
        }

        public void ActivateSkill()
        {
            if (!HasSkillAlready) return;
            SkillManager.Instance.ActivateSkill(_skill.SkillType, transform, OwnerClientId);
            SkillsUI.Instance.SetSkillToNull();
            _hasSkillAlready = false;
            Debug.Log("Skill Used:" + _skill.SkillType);
        }
    }
}