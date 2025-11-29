using System.Collections.Generic;
using _Projects.Scripts.SkillSystem;
using _Projects.Scripts.SkillSystem.Serializables;
using Unity.Netcode;
using UnityEngine;

namespace _Project.SkillSystem
{
    public class SkillManager : NetworkBehaviour
    {
        public static SkillManager Instance { get; private set; }
        [SerializeField] private MysteryBoxSkillSO[] _skills;
        private Dictionary<SkillTypes, MysteryBoxSkillSO> _skillDictionary;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            _skillDictionary = new Dictionary<SkillTypes, MysteryBoxSkillSO>();
            foreach (MysteryBoxSkillSO skill in _skills)
            {
                _skillDictionary[skill.SkillType] = skill;
            }
        }

        public void ActivateSkill(SkillTypes skillType, Transform playerTransform, ulong spawnerClientId)
        {
            SkillDataSerializable skillDataSerializable = new SkillDataSerializable(playerTransform.position,
                playerTransform.rotation
                , skillType, playerTransform.GetComponent<NetworkObject>());
            if (!IsServer)
            {
                RequestSpawnRPC(skillDataSerializable, spawnerClientId);
                return;
            }

            SpawnSkill(skillDataSerializable, spawnerClientId);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void RequestSpawnRPC(SkillDataSerializable skill, ulong spawnerClientId)
        {
            SpawnSkill(skill, spawnerClientId);
        }

        private void SpawnSkill(SkillDataSerializable skill, ulong spawnerClientId)
        {
            if (!_skillDictionary.TryGetValue(skill.SkillType, out MysteryBoxSkillSO skillData))
            {
                Debug.LogError($"Spawn Skill: {skill.SkillType} not found");
                return;
            }

            if (skill.SkillType == SkillTypes.Mine)
            {
            }
            else
            {
                Spawn(skill, spawnerClientId, skillData);
            }
        }

        public void Spawn(SkillDataSerializable skillDataSerializable, ulong spawnerClientId, 
            MysteryBoxSkillSO skillData)
        {
            if (!IsServer) return;
            Transform skillInstance = Instantiate(skillData.SkillData.SkillPrefab);
            skillInstance.SetPositionAndRotation(skillDataSerializable.Position,
                skillDataSerializable.Rotation);
            var networkObject = skillInstance.GetComponent<NetworkObject>();
            networkObject.SpawnWithOwnership(spawnerClientId);
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(spawnerClientId, out var client))
            {
                if (skillData.SkillType != SkillTypes.Rocket)
                {
                    networkObject.TrySetParent(client.PlayerObject);
                }
                else
                {
                }

                if (skillData.SkillData.ShouldBeAttachedToParent)
                {
                    networkObject.transform.localPosition = Vector3.zero;
                }

                PositionDataSerializable positionDataSerializable = new PositionDataSerializable(
                    skillInstance.transform.localPosition + skillData.SkillData.SkillOffset);
                UpdateSkillPositionRPC(networkObject.NetworkObjectId, positionDataSerializable);

                if (!skillData.SkillData.ShouldBeAttachedToParent)
                {
                    networkObject.TryRemoveParent();
                }
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void UpdateSkillPositionRPC(ulong objectId, PositionDataSerializable positionDataSerializable)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var spawnedObject))
            {
                spawnedObject.transform.localPosition = positionDataSerializable.Position;
            }
        }
    }
}