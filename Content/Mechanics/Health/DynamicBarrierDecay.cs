namespace WellRoundedBalance.Mechanics.Health
{
    public class DynamicBarrierDecay : MechanicBase<DynamicBarrierDecay>
    {
        // lab ver
        public override string Name => ":: Mechanics :: Dynamic Barrier Decay";

        [ConfigField("Current Barrier Coefficient", "Formula for barrier decay: Current Barrier * Current Barrier Coefficient + (Full Health + Full Shield) * Full Combined Health Coefficient", 0.07f)]
        public static float currentBarrierCoefficient;

        [ConfigField("Full Combined Health Coefficient", "Decimal. Formula for barrier decay: Current Barrier * Current Barrier Coefficient + (Full Health + Full Shield) * Full Combined Health Coefficient", 0.008f)]
        public static float fullCombinedHealthCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
        }

        private void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            self.barrierDecayRate = self.healthComponent.barrier * currentBarrierCoefficient + self.healthComponent.fullCombinedHealth * fullCombinedHealthCoefficient;
            orig(self);
        }
    }
}