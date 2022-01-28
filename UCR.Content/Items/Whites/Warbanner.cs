using R2API;
using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class Warbanner
    {
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
                if (sender.HasBuff(RoR2Content.Buffs.Warbanner.buffIndex))
                {
                    args.baseDamageAdd += Main.WarbannerDamage.Value * stack;
                }
            }
        }
        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8),
                x => x.MatchLdcR4(8)
            );
            c.Next.Operand = Main.WarbannerRadius.Value;
            c.Index += 1;
            c.Next.Operand = Main.WarbannerRadiusStack.Value;
        }
        public static void ChangeRadiusTP(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8),
                x => x.MatchLdcR4(8)
            );
            c.Next.Operand = Main.WarbannerRadius.Value;
            c.Index += 1;
            c.Next.Operand = Main.WarbannerRadiusStack.Value;
        }
    }
}
