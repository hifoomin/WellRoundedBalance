using WellRoundedBalance.Items.Yellows;

namespace WellRoundedBalance.Items.Whites
{
    public class TriTipDagger : ItemBase<TriTipDagger>
    {
        public static float Chance;

        public override string Name => ":: Items : Whites :: Tri Tip Dagger";
        public override ItemDef InternalPickup => RoR2Content.Items.BleedOnHit;

        public override string PickupText => $"Gain +{d(bleedChance)} chance to bleed enemies on hit.";

        public override string DescText =>
            StackDesc(bleedChance, bleedChanceStack, init => $"<style=cIsDamage>{d(init)}</style>{{Stack}} chance to <style=cIsDamage>bleed</style> an enemy up to <style=cIsDamage>" + baseBleedCapPerTarget + "</style>" +
            (bleedCapPerTargetPerStack > 0 ? " <style=cStack>(+" + bleedCapPerTargetPerStack + " per stack)</style>" : "") +
            " times for <style=cIsDamage>240%</style> base damage.", d);

        [ConfigField("Bleed Chance", 0.09f)]
        public static float bleedChance;

        [ConfigField("Bleed Chance per Stack", 0.09f)]
        public static float bleedChanceStack;

        [ConfigField("Damage is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float bleedChanceIsHyperbolic;

        [ConfigField("Base Bleed Cap Per Target", "", 8)]
        public static int baseBleedCapPerTarget;

        [ConfigField("Bleed Cap Per Target Per Stack", "", 4)]
        public static int bleedCapPerTargetPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.DotController.InflictDot_GameObject_GameObject_DotIndex_float_float_Nullable1 += DotController_InflictDot_GameObject_GameObject_DotIndex_float_float_Nullable1;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void DotController_InflictDot_GameObject_GameObject_DotIndex_float_float_Nullable1(On.RoR2.DotController.orig_InflictDot_GameObject_GameObject_DotIndex_float_float_Nullable1 orig, GameObject victimObject, GameObject attackerObject, DotController.DotIndex dotIndex, float duration, float damageMultiplier, uint? maxStacksFromAttacker)
        {
            var attackerBody = attackerObject.GetComponent<CharacterBody>();
            if (attackerBody)
            {
                var inventory = attackerBody.inventory;
                if (inventory)
                {
                    var stack = inventory.GetItemCount(RoR2Content.Items.BleedOnHit);
                    var spleen = inventory.GetItemCount(RoR2Content.Items.BleedOnHitAndExplode);
                    if (dotIndex == DotController.DotIndex.Bleed)
                    {
                        var triBleedCap = baseBleedCapPerTarget + bleedCapPerTargetPerStack * (stack - 1);
                        int spleenBleedCap = 0;
                        if (spleen > 0)
                        {
                            spleenBleedCap = Shatterspleen.baseBleedCapPerTarget + Shatterspleen.bleedCapPerTargetPerStack * (spleen - 1);
                        }
                        maxStacksFromAttacker = (uint)triBleedCap + (uint)spleenBleedCap;
                    }
                }
            }
            orig(victimObject, attackerObject, dotIndex, duration, damageMultiplier, maxStacksFromAttacker);
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            var inventory = self.inventory;
            if (inventory)
            {
                var stack = self.inventory.GetItemCount(RoR2Content.Items.BleedOnHit);
                if (self.inventory && stack > 0) self.bleedChance += StackAmount(bleedChance, bleedChanceStack, stack, bleedChanceIsHyperbolic) * 100f - 10f;
            }
        }
    }
}