using System;
using Unity.Netcode;

namespace _Projects.Scripts.Serializables
{
    public struct PlayerDataSerializable : INetworkSerializeByMemcpy, IEquatable<PlayerDataSerializable>,IEquatable<ulong>
    {
        public ulong ClientId;
        public bool Equals(PlayerDataSerializable other)
        {
            return ClientId == other.ClientId;
        }

        public bool Equals(ulong clientId)
        {
            return ClientId == clientId;
        }
    }
}