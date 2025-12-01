 
using Game.Player;

public interface IDamageable  
{
   void Damage(PlayerVehicleController vehicle);
   ulong GetKillerClientID();
   int GetRespawnTimer { get; }
}
