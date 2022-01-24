using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun
{
    public static class HarvestersScythe
    {
        public static void ChangeCrit(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(5f),
                x => x.MatchAdd()
            );
            // ok how the FUCK does this work lmao theres so many of the same instructions
            // id actually need to know borbo's magic of knowing what the V_ values are
            // or just do the workaround of changing this to 0 and adding behavior in recalcstats :wittyresponse
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
                    if (Main.HarvestersCritStack.Value)
                    {
                        args.critAdd += Main.HarvestersCrit.Value * stack;
                    }
                    else
                    {
                        args.critAdd += Main.HarvestersCrit.Value;
                    }
                }
            }
        }
        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(4f),
                x => x.MatchLdloc(2),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(4f)
            );
            c.Next.Operand = Main.HarvestersHealBase.Value;
            c.Index += 3;
            c.Next.Operand = Main.HarvestersHealStack.Value;
        }
    }
}
