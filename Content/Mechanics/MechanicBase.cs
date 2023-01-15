namespace WellRoundedBalance.Mechanic
{
    public abstract class MechanicBase
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