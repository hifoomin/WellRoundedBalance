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
            StackCrit = ConfigOption(false, "Stack Crit Chance?", "Vanilla is false");
            BaseHealing = ConfigOption(8f, "Flat Healing", "Vanilla is 8");
            StackHealing = ConfigOption(4f, "Stack Flat Healing", "Per Stack. Vanilla is 4");
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

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(5f),
                x => x.MatchAdd()))
            {
                c.Index += 1;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Harvester's Scythe hook");
            }
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

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(4f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(4f)))
            {
                c.Next.Operand = BaseHealing - StackHealing;
                c.Index += 3;
                c.Next.Operand = StackHealing;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Harvester's Scythe Healing hook");
            }
        }
    }
}