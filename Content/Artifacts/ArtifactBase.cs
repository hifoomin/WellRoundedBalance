using BepInEx.Configuration;

namespace WellRoundedBalance.Artifacts
{
    public abstract class ArtifactBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBArtifactConfig;
    }
}