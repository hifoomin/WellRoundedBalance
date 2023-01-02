namespace WellRoundedBalance.Mechanic.Health
{
    public class LowHealthThreshold : GlobalBase
    {
        public override string Name => ": Global :: Health";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            // new ILHook(typeof(CharacterBody).GetMethod("get_isHealthLow"), ChangeThreshold);
            On.RoR2.HealthComponent.Awake += ChangeThreshold;
        }

        private void ChangeThreshold(On.RoR2.HealthComponent.orig_Awake orig, HealthComponent self)
        {
            HealthComponent.lowHealthFraction = 0.25f;
            orig(self);
        }
    }
}