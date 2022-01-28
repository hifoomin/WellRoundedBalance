using R2API;
using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class RepulsionArmorPlate
    {
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.ArmorPlate);
                if (stack > 0)
                {
                    args.armorAdd += Main.RapArmor.Value * stack;
                }
            }
        }

        public static void ChangeReduction(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(5)
            );
            c.Index += 2;
            c.Next.Operand = Main.RapFlatDmgDecrease.Value;
        }

        public static void ChangeMinimum(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                //x => x.MatchLdflda<HealthComponent>("itemCounts"),
                //x => x.MatchLdfld<HealthComponent>("armorPlate"),
                x => x.MatchLdcI4(0),
                x => x.MatchBle(out _),
                x => x.MatchLdcR4(1)
            );
            c.Index += 2;
            c.Next.Operand = Main.RapMinimumDmgTaken.Value;
        }
    }
}
