using R2API;

namespace UltimateCustomRun
{
    public abstract class SurvivorBase
    {
        public abstract string Name { get; }

        public T ConfigOption<T>(T value, string name, string description)
        {
            return Main.UCRConfig.Bind<T>(Name, name, value, description).Value;
        }

        public abstract void Hooks();

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        //List<string> Names = new List<string>();

        public virtual void Init()
        {
            Hooks();
            Main.UCRLogger.LogInfo("Added " + Name);
        }
    }
}