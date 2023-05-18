using BepInEx.Configuration;

namespace WellRoundedBalance.Elites
{
    public abstract class EliteBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBEliteConfig;
        public static List<string> eliteList = new();

        public override void Init()
        {
            base.Init();
            eliteList.Add(Name);
        }
    }
}