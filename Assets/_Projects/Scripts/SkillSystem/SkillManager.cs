using System;
using System.Collections.Generic;
using _Project.Serializables;
using _Projects.Player;
using Cysharp.Threading.Tasks;
using _Projects.Serializables;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.SkillSystem
{
    public class SkillManager : NetworkBehaviour
    {
        public static SkillManager Instance { get; private set; }

        public Action OnMineCountReduced;
        [SerializeField] private MysteryBoxSkillSO[] _skills;
        [SerializeField] private LayerMask _groundLayer;

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

        private async void SpawnSkill(SkillDataSerializable skillDataSerializable, ulong spawnerClientId)
        {
            if (!_skillDictionary.TryGetValue(skillDataSerializable.SkillType, out MysteryBoxSkillSO skillData))
            {
                Debug.LogError($"Spawn Skill: {skillDataSerializable.SkillType} not found");
                return;
            }

            if (skillDataSerializable.SkillType == SkillTypes.Mine)
            {
                Vector3 spawnPosition = skillDataSerializable.Position;
                Vector3 spawnDirection = skillDataSerializable.Rotation * Vector3.forward;

                for (int i = 0; i < skillData.SkillData.SpawnAmountOrTimer; i++)
                {
                    Vector3 offset = spawnDirection * (i * 3f);
                    skillDataSerializable.SetPosition(spawnPosition + offset);
                    Spawn(skillDataSerializable, spawnerClientId, skillData);
                    await UniTask.Delay(200);
                    OnMineCountReduced?.Invoke();
                }
            }
            else
            {
                Spawn(skillDataSerializable, spawnerClientId, skillData);
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
                    PlayerSkillController playerSkillController =
                        client.PlayerObject.GetComponent<PlayerSkillController>();
                    networkObject.transform.localPosition = playerSkillController.GetRocketLauncherPosition();
                    return;
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

                if (skillData.SkillType == SkillTypes.FakeBox)
                {
                    float ground = GetGroundHeight(skillData, skillInstance.transform.position);
                    positionDataSerializable = new PositionDataSerializable(
                        new Vector3(skillInstance.transform.position.x, ground, skillInstance.transform.position.z));
                    UpdateSkillPositionRPC(networkObject.NetworkObjectId, positionDataSerializable, true);
                }
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void UpdateSkillPositionRPC(ulong objectId, PositionDataSerializable positionDataSerializable,
            bool isSpecialPosition = false)
        {
            if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var spawnedObject))
                return;
            if (isSpecialPosition)
                spawnedObject.transform.position = positionDataSerializable.Position;
            else
                spawnedObject.transform.localPosition = positionDataSerializable.Position;
        }

        private float GetGroundHeight(MysteryBoxSkillSO skill, Vector3 position)
        {
            if (Physics.Raycast(new Vector3(position.x, position.y, position.z), Vector3.down,
                    out RaycastHit raycastHit, 10f, _groundLayer))
            {
                return skill.SkillData.SkillOffset.y;
            }

            return skill.SkillData.SkillOffset.y;
        }
    }
}