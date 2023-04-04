using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class AlienHead : ItemBase<AlienHead>
    {
        public override string Name => ":: Items ::: Reds :: Alien Head";
        public override ItemDef InternalPickup => RoR2Content.Items.AlienHead;

        public override string PickupText => "Reduces cooldowns for your skills.";

        public override string DescText => "<style=cIsUtility>Reduce skill cooldowns</style> by <style=cIsUtility>" + flatCooldownReduction + "s</style> and <style=cIsUtility>" + d(percentCooldownReduction) + "</style> <style=cStack>(+" + d(percentCooldownReduction) + " per stack)</style>.";

        [ConfigField("Percent Cooldown Reduction", "Decimal.", 0.3f)]
        public static float percentCooldownReduction;

        [ConfigField("Flat Cooldown Reduction", 0.5f)]
        public static float flatCooldownReduction;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.AlienHead);
                if (stack > 0)
                {
                    args.cooldownReductionAdd += flatCooldownReduction;
                }
            }
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.75f),
                    x => x.MatchMul()))
            {
                c.Next.Operand = 1 - percentCooldownReduction;
            }
            else
            {
                Logger.LogError("Failed to apply Alien Head Cooldown Reduction hook");
            }
        }
    }
}