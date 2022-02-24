namespace UltimateCustomRun
{
    public abstract class EnemyBase
    {
        public abstract string Name { get; }
        public virtual bool isEnabled { get; } = true;

        public T ConfigOption<T>(T value, string name, string description)
        {
            return Main.UCRConfig.Bind<T>(Name, name, value, description).Value;
        }

        public abstract void Hooks();

        public virtual void Init()
        {
            Hooks();
            Main.UCRLogger.LogInfo("Added " + Name);
        }
    }
}