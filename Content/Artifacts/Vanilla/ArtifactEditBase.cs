using BepInEx.Configuration;

namespace WellRoundedBalance.Artifacts.Vanilla
{
    public abstract class ArtifactEditBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBArtifactEditConfig;
        public static List<string> artifactEditList = new();

        public override void Init()
        {
            base.Init();
            artifactEditList.Add(Name);
        }
    }
}