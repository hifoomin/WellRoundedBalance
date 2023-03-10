using BepInEx.Configuration;

namespace WellRoundedBalance.Mechanics
{
    public abstract class MechanicBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBMechanicConfig;
    }
}