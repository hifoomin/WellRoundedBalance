using R2API;
using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class LensMakersGlasses
    {
        public static void ChangeCrit(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<CharacterBody>("levelCrit")
            );
            c.Index += 8;
            c.Next.Operand = Main.LensMakersCrit.Value;
            // I dont actually know why the standard method doesnt work
            // Thanks to uhh someone for this instead
        }
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.CritGlasses);
                if (stack > 0)
                {
                    // waiting for RecalcStats update if possible to add critdamagemult :thonk: += Main.LensMakersCritDamage.Value * stack;
                }
            }
        }
    }
}
