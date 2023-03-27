using BepInEx.Configuration;

namespace WellRoundedBalance.Gamemodes
{
    public abstract class GamemodeBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBGamemodeConfig;
    }
}