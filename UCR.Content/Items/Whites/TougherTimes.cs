using R2API;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class TougherTimes
    {
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Bear);
                if (stack > 0)
                {
                    args.armorAdd += Main.TougherTimesArmor.Value * stack;
                }
            }
        }
        public static void ChangeBlock(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                 x => x.MatchLdcI4(0),
                 x => x.Match(OpCodes.Ble_S),
                 x => x.MatchLdcR4(15f)
            );
            c.Index += 2;
            c.Next.Operand = Main.TougherTimesBlockChance.Value;
        }
    }
}
