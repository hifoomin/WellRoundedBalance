using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun
{
    static class HarvestersScythe
    {
        public static void ChangeCrit(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(5f),
                x => x.MatchAdd()
            );
            c.Index += 1;
            c.Next.Operand = Main.HarvestersCrit.Value;
        }
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.HealOnCrit);
                if (stack > 0)
                {
                    args.critAdd += Main.HarvestersCrit.Value * (stack - 1);
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
