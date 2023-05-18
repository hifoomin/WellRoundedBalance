using BepInEx.Configuration;

namespace WellRoundedBalance.Allies
{
    public abstract class AllyBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBAllyConfig;
        public static List<string> allyList = new();

        public override void Init()
        {
            base.Init();
            allyList.Add(Name);
        }
    }
}