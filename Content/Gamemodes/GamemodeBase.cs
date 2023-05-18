using BepInEx.Configuration;

namespace WellRoundedBalance.Gamemodes
{
    public abstract class GamemodeBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBGamemodeConfig;
        public static List<string> gamemodeList = new();

        public override void Init()
        {
            base.Init();
            gamemodeList.Add(Name);
        }
    }
}