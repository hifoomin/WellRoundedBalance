namespace WellRoundedBalance.Enemies
{
    public abstract class EnemyBase
    {
        public abstract string Name { get; }
        public virtual bool isEnabled { get; } = true;

        public abstract void Hooks();

        public virtual void Init()
        {
            ConfigManager.HandleConfigAttributes(this.GetType(), Name, Main.WRBEnemyConfig);
            Hooks();
        }
    }
}