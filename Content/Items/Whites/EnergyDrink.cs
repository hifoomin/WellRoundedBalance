using System;

namespace WellRoundedBalance.Items.Whites
{
    public class EnergyDrink : ItemBase<EnergyDrink>
    {
        public override string Name => ":: Items : Whites :: Energy Drink";
        public override ItemDef InternalPickup => RoR2Content.Items.SprintBonus;

        public override string PickupText => "Increase sprint speed by +" + d(speedBonus) + ".";

        public override string DescText => "<style=cIsUtility>Sprint speed</style> is improved by <style=cIsUtility>" + d(speedBonus) + "</style> <style=cStack>(+" + d(speedBonus) + " per stack)</style>.";

        [ConfigField("Really Increase Sprint Speed?", "Makes Energy Drink increase the Sprinting Speed Multiplier instead of speed while sprinting.", true)]
        public static bool really;

        [ConfigField("Speed Bonus", "Decimal.", 0.15f)]
        public static float speedBonus;

        // GET BETTER GET GET BETTER GET BETTER GET LET ME SING FOR THIS GET BETTER GET GET BETTER GET BETTER GET LET ME SING FOR THIS!

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            if (really)
                RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SprintBonus);
                args.sprintSpeedAdd += speedBonus * stack;
            }
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.25f),
                x => x.MatchLdloc(out _)))
            {
                c.Index += 2;
                c.EmitDelegate<Func<float, float>>((orig) =>
                {
                    return really ? 0f : speedBonus;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Energy Drink Deletion hook");
            }
        }
    }
}