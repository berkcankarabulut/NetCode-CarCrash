using Unity.Netcode;
using UnityEngine;

namespace _Projects.Scripts.SkillSystem.Serializables
{
    public struct SkillDataSerializable : INetworkSerializeByMemcpy
    {
        private Vector3 _position;
        private Quaternion _rotation;
        private SkillTypes _skillType;
        private NetworkObject _networkObject;

        public Vector3 Position => _position;
        public Quaternion Rotation => _rotation;
        public SkillTypes SkillType => _skillType;
        public NetworkObject NetworkObject => _networkObject;

        public void SetPosition(Vector3 position)
        {
            _position = position;
        }
        public SkillDataSerializable(Vector3 position, Quaternion rotation, SkillTypes skillType,
            NetworkObject networkObject)
        {
            _position = position;
            _rotation = rotation;
            _skillType = skillType;
            _networkObject = networkObject;
        }
    }
}