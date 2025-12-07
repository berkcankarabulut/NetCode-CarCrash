 

using System;
using Unity.Collections;
using Unity.Netcode;

namespace _Projects.Scripts.Serializables
{
    public struct LeaderBoardEntitesSerializabes : INetworkSerializeByMemcpy, IEquatable<LeaderBoardEntitesSerializabes>
    {
        public ulong ClientId { get; }
        public FixedString32Bytes PlayerName { get; }
        public int Score { get; }

        public LeaderBoardEntitesSerializabes(ulong clientId, string playerName, int score)
        {
            ClientId = clientId;
            PlayerName = playerName;
            Score = score;
        }
        public bool Equals(LeaderBoardEntitesSerializabes other)
        {
            return ClientId == other.ClientId
                   && PlayerName.Equals(other.PlayerName)
                   && Score == other.Score;
        }
    }
}