using System;
using Unity.Netcode;

namespace _Projects.Scripts.Serializables
{
    public struct PlayerDataSerializable : INetworkSerializeByMemcpy, IEquatable<PlayerDataSerializable>
    {
        public ulong ClientId;
        public int ColorId;

        public PlayerDataSerializable(ulong clientId, int colorId)
        {
            ClientId = clientId;
            ColorId = colorId;
        }

        public bool Equals(PlayerDataSerializable other)
        {
            return ClientId == other.ClientId;
        }
    }
}