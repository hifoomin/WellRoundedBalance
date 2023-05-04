using BepInEx.Configuration;

namespace WellRoundedBalance.Survivors
{
    public abstract class SurvivorBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBSurvivorConfig;
    }
}