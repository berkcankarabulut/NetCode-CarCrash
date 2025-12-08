using _Projects.Helpers.Const;
using _Projects.Player;

namespace _Projects.Collect
{
    public interface ICollectible
    {
        void Collect(PlayerSkillController controller, CameraShake cameraShake);
        void CollectRpc();
    }
}