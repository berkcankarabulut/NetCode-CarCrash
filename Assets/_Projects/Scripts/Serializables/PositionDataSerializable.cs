
using Unity.Netcode;
using UnityEngine;

namespace _Projects.Serializables
{
    public struct PositionDataSerializable : INetworkSerializeByMemcpy
    {
        public Vector3 Position;

        public PositionDataSerializable(Vector3 position)
        {
            this.Position = position;
        }
    }
}