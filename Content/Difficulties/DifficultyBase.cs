using BepInEx.Configuration;

namespace WellRoundedBalance.Difficulties
{
    public abstract class DifficultyBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBDifficultyConfig;
        public abstract string InternalDiffToken { get; }
        public abstract string DescText { get; }
    }
}