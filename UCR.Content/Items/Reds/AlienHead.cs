using R2API;
using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class AlienHead
    {
        public static void ChangeCDR(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.75f),
                x => x.MatchMul()
            );
            c.Next.Operand = 1f - Main.AlienHeadCDR.Value;
        }
        // PLEASE HELP TO FIX
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.AlienHead);
                if (stack > 0)
                {
                    args.cooldownReductionAdd += Main.AlienHeadFlatCDR.Value;
                }
            }
        }
        // PLEASE HELP TO FIX
    }
}
