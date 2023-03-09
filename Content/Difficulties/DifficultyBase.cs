namespace WellRoundedBalance.Difficulties
{
    public abstract class DifficultyBase
    {
        public abstract string Name { get; }
        public abstract string InternalDiffToken { get; }
        public abstract string DescText { get; }
        public virtual bool isEnabled { get; } = true;

        public abstract void Hooks();

        public string d(float f) => (f * 100f).ToString() + "%";

        public virtual void Init()
        {
            ConfigManager.HandleConfigAttributes(this.GetType(), Name, Main.WRBDifficultyConfig);
            Hooks();
            LanguageAPI.Add(InternalDiffToken.ToUpper(), DescText);
        }
    }
}