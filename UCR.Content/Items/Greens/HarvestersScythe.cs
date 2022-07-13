using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Greens
{
    public class HarvestersScythe : ItemBase
    {
        public static float Crit;
        public static bool StackCrit;
        public static float BaseHealing;
        public static float StackHealing;

        public override string Name => ":: Items :: Greens :: Harvesters Scythe";
        public override string InternalPickupToken => "healOnCrit";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Gain <style=cIsDamage>" + Crit + "% critical chance</style>" +
                                           (StackCrit ? " <style=cStack>(+" + Crit + "% per stack)</style>." : ".") +
                                           " <style=cIsDamage>Critical strikes</style> <style=cIsHealing>heal</style> for <style=cIsHealing>" + BaseHealing + "</style> <style=cStack>(+" + StackHealing + " per stack)</style> <style=cIsHealing>health</style>.";

        public override void Init()
        {
            Crit = ConfigOption(5f, "Crit Chance", "Vanilla is 5");
            ROSOption("Greens", 0f, 20f, 1f, "2");
            StackCrit = ConfigOption(false, "Stack Crit Chance?", "Vanilla is false");
            ROSOption("Greens", 0f, 5f, 0.01f, "2");
            BaseHealing = ConfigOption(8f, "Base Healing", "Vanilla is 8");
            ROSOption("Greens", 0f, 20f, 0.5f, "2");
            StackHealing = ConfigOption(4f, "Stack Healing", "Per Stack. Vanilla is 4");
            ROSOption("Greens", 0f, 20f, 0.5f, "2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeCrit;
            IL.RoR2.GlobalEventManager.OnCrit += ChangeHealing;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void ChangeCrit(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(5f),
                x => x.MatchAdd()
            );
            // ok how the FUCK does this work lmao theres so many of the same instructions
            // id actually need to know borbo's magic of knowing what the V_ values are
            // or just do the workaround of changing this to 0 and adding behavior in recalcstats :WittyComeback:
            c.Index += 1;
            c.Next.Operand = 0f;
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.HealOnCrit);
                if (stack > 0)
                {
                    args.critAdd += StackCrit ? Crit * stack : Crit;
                }
            }
        }

        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(4f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(4f)
            );
            c.Next.Operand = BaseHealing - StackHealing;
            c.Index += 3;
            c.Next.Operand = StackHealing;
        }
    }
}