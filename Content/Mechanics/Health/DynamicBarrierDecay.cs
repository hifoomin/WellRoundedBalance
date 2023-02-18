namespace WellRoundedBalance.Mechanics.Health
{
    public class DynamicBarrierDecay : MechanicBase
    {
        // lab ver
        public override string Name => ":: Mechanics :: Dynamic Barrier Decay";

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
            self.barrierDecayRate = self.healthComponent.barrier * 0.071f + self.healthComponent.fullCombinedHealth * 0.008f;
            orig(self);
        }
    }
}