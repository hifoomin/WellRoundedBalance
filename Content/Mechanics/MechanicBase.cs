namespace WellRoundedBalance.Mechanics
{
    public abstract class MechanicBase
    {
        public abstract string Name { get; }
        public virtual bool isEnabled { get; } = true;

        public abstract void Hooks();

        public virtual void Init()
        {
            ConfigManager.HandleConfigAttributes(this.GetType(), Name, Main.WRBMechanicConfig);
            Hooks();
        }
    }
}