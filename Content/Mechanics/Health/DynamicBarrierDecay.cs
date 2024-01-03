using WellRoundedBalance.Items.Reds;

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
            float barrierDecayRate = 0f;

            if (self.inventory)
            {
                var stack = self.inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                if (stack > 0 && self.outOfDanger)
                {
                    barrierDecayRate = 0f;
                }
                else
                {
                    barrierDecayRate = self.healthComponent.barrier * currentBarrierCoefficient + self.healthComponent.fullCombinedHealth * fullCombinedHealthCoefficient;
                }
            }

            self.barrierDecayRate = barrierDecayRate;
            orig(self);
        }
    }

    public static class Jank
    {
        public static void Init()
        {
            if (DynamicBarrierDecay.instance.isEnabled == false && Aegis.instance.isEnabled)
            {
                On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
            }
        }

        private static void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            if (self.inventory)
            {
                var stack = self.inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                if (stack > 0 && self.outOfDanger)
                {
                    self.barrierDecayRate = 0f;
                }
            }
            orig(self);
        }
    }
}