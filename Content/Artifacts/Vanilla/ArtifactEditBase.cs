using BepInEx.Configuration;

namespace WellRoundedBalance.Artifacts.Vanilla
{
    public abstract class ArtifactEditBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBArtifactConfig;
    }
}