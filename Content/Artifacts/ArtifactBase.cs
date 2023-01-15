namespace WellRoundedBalance.Artifacts
{
    public abstract class ArtifactBase
    {
        public abstract string Name { get; }
        public virtual bool isEnabled { get; } = true;

        public abstract void Hooks();

        public virtual void Init()
        {
            Hooks();
        }
    }
}