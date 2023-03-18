namespace WellRoundedBalance.Mechanics.Health
{
    public class LowHealthThreshold : MechanicBase
    {
        public override string Name => ":: Mechanics :: Low Health Threshold";

        [ConfigField("Low Health Fraction", "Decimal. Affects low health visuals and all other mods using it!", 0.25f)]
        public static float lowHealthFraction;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.HealthComponent.Awake += ChangeThreshold;
            On.RoR2.UI.HealthBar.UpdateBarInfos += HealthBar_UpdateBarInfos;
        }

        private void HealthBar_UpdateBarInfos(On.RoR2.UI.HealthBar.orig_UpdateBarInfos orig, RoR2.UI.HealthBar self)
        {
            orig(self);
            var hc = self.source;
            if (hc)
            {
                var bar = self.barInfoCollection.lowHealthUnderBarInfo;
                bool underHalf = (hc.health / hc.shield) / hc.fullCombinedHealth <= 0.5f;
                bar.enabled = self.hasLowHealthItem && underHalf;

                bar.normalizedXMax = 0.5f * (1f - hc.GetHealthBarValues().curseFraction);
            }
        }

        private void ChangeThreshold(On.RoR2.HealthComponent.orig_Awake orig, HealthComponent self)
        {
            HealthComponent.lowHealthFraction = lowHealthFraction;
            orig(self);
        }
    }
}