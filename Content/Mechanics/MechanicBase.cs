using BepInEx.Logging;

namespace WellRoundedBalance.Mechanics
{
    public abstract class MechanicBase
    {
        public abstract string Name { get; }
        public virtual bool isEnabled { get; } = true;
        public static ManualLogSource Logger => Main.WRBLogger;

        public abstract void Hooks();

        public virtual void Init()
        {
            ConfigManager.HandleConfigAttributes(GetType(), Name, Main.WRBMechanicConfig);
            Hooks();
        }
    }
}