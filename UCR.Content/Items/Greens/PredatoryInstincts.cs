using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun
{
    public static class PredatoryInstincts
    {
        public static void ChangeAS(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("GetBuffCount"),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.12f)
            );
            c.Index += 2;
            c.Next.Operand = Main.PredatoryAS.Value;
        }

        public static void ChangeCap(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(1),
                x => x.MatchLdcI4(1),
                x => x.MatchLdloc(1),
                x => x.MatchLdcI4(2)
            );
            c.Index += 1;
            c.Next.Operand = Main.PredatoryBaseCap.Value;
            c.Index += 2;
            c.Next.Operand = Main.PredatoryStackCap.Value;
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.AttackSpeedOnCrit);
                int buff = sender.GetBuffCount(RoR2Content.Buffs.AttackSpeedOnCrit);
                if (stack > 0)
                {
                    args.critAdd += Main.PredatoryCritStack.Value ? Main.PredatoryCrit.Value * stack : Main.PredatoryCrit.Value;

                    if (buff > 0)
                    {
                        args.moveSpeedMultAdd += Main.PredatorySpeedStack.Value ? Main.PredatorySpeed.Value * buff * stack : Main.PredatorySpeed.Value * buff;
                    }
                }
            }
        }
        // NONE OF THESE WORK PLEASE HELP TO FIX
    }
}
