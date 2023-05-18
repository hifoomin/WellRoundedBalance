using BepInEx.Configuration;

namespace WellRoundedBalance.Mechanics
{
    public abstract class MechanicBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBMechanicConfig;
        public static List<string> mechanicList = new();

        public override void Init()
        {
            base.Init();
            mechanicList.Add(Name);
        }
    }
}