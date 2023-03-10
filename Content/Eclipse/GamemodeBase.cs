using BepInEx.Configuration;

namespace WellRoundedBalance.Eclipse
{
    public abstract class GamemodeBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBGamemodeConfig;
    }
}