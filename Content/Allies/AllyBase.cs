using BepInEx.Configuration;

namespace WellRoundedBalance.Allies
{
    public abstract class AllyBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBAllyConfig;
    }
}