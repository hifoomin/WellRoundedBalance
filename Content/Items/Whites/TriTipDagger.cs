using WellRoundedBalance.Items.Yellows;
using WellRoundedBalance.Misc;

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

        [ConfigField("Base Bleed Chance", 0.1f)]
        public static float bleedChance;

        [ConfigField("Bleed Chance per Stack", 0.1f)]
        public static float bleedChanceStack;

        [ConfigField("Damage is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float bleedChanceIsHyperbolic;

        [ConfigField("Base Bleed Cap Per Target", "", 4)]
        public static int baseBleedCapPerTarget;

        [ConfigField("Bleed Cap Per Target Per Stack", "", 4)]
        public static int bleedCapPerTargetPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            RecalculateEvent.RecalculateBleedCap += (object sender, RecalculateEventArgs args) =>
            {
                if (args.BleedCap)
                {
                    var body = args.BleedCap.body;
                    if (body)
                    {
                        var inventory = args.BleedCap.body.inventory;
                        if (inventory)
                        {
                            var stack = inventory.GetItemCount(RoR2Content.Items.BleedOnHit);
                            if (stack > 0)
                            {
                                args.BleedCap.bleedCapAdd += baseBleedCapPerTarget + bleedCapPerTargetPerStack * (stack - 1);
                            }
                        }
                    }
                }
            };
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