using BepInEx.Configuration;

namespace WellRoundedBalance.Enemies
{
    public abstract class EnemyBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBEnemyConfig;
    }
}