namespace WellRoundedBalance.Items.Whites
{
    public class TriTipDagger : ItemBase
    {
        public static float Chance;

        public override string Name => ":: Items : Whites :: Tri Tip Dagger";
        public override ItemDef InternalPickup => RoR2Content.Items.BleedOnHit;

        public override string PickupText => $"Gain +{bleedChance}% chance to bleed enemies on hit.";

        public override string DescText =>
            StackDesc(bleedChance, bleedChanceStack, init => $"<style=cIsDamage>{d(init)}</style>{{Stack}} chance to <style=cIsDamage>bleed</style> an enemy for <style=cIsDamage>240%</style> base damage.", d);

        [ConfigField("Bleed Chance", 0.09f)]
        public static float bleedChance;

        [ConfigField("Bleed Chance per Stack", 0.09f)]
        public static float bleedChanceStack;

        [ConfigField("Damage is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float bleedChanceIsHyperbolic;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            int stack = self.inventory.GetItemCount(RoR2Content.Items.BleedOnHit);
            if (self.inventory && stack > 0) self.bleedChance += StackAmount(bleedChance, bleedChanceStack, stack, bleedChanceIsHyperbolic) * 100f - 10f;
        }
    }
}