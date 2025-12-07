 
using _Projects.Player;

public interface IDamageable  
{
   void Damage(PlayerVehicleController vehicle, string playerName);
   ulong GetKillerClientID();
   int GetRespawnTimer { get; }
   int GetDamageAmount { get; }
   string GetKillerName();
}
