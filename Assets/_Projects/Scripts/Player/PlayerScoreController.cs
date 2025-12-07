using Unity.Netcode;

namespace _Projects.Player
{
    public class PlayerScoreController : NetworkBehaviour
    {
        public NetworkVariable<int> PlayerScore = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
        );

        public void AddScore(int score)
        {
            PlayerScore.Value += score;
        }
    }
}