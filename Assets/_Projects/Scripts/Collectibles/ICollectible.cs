using Game.Player;

namespace _Project.Collect
{
    public interface ICollectible
    {
        void Collect(PlayerSkillController controller);
        void CollectRpc();
    }
}