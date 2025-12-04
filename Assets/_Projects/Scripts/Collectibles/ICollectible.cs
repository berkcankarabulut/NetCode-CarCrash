using _Projects.Player;

namespace _Projects.Collect
{
    public interface ICollectible
    {
        void Collect(PlayerSkillController controller);
        void CollectRpc();
    }
}