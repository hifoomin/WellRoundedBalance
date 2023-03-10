using BepInEx.Configuration;

namespace WellRoundedBalance.Elites
{
    public abstract class EliteBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBEliteConfig;
    }
}