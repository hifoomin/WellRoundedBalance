using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun
{
    public static class BerzerkersPauldron
    {
        public static void ChangeKillCount(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_multiKillCount"),
                x => x.MatchLdcI4(4)
            );
            c.Index += 2;
            c.Next.Operand = Main.BerzerkersKillsReq.Value;
            // WHY DOES THIS NOT WORK
        }
        public static void ChangeBuffDuration(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "WarCryBuff"),
                x => x.MatchLdcR4(2f),
                x => x.MatchLdcR4(4f)
            );
            c.Index += 1;
            c.Next.Operand = Main.BerzerkersDurationBase.Value;
            c.Index += 1;
            c.Next.Operand = Main.BerzerkersDurationStack.Value;
        }
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var buff = sender.HasBuff(RoR2Content.Buffs.WarCryBuff);
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.WarCryOnMultiKill);
                if (stack > 0 && buff)
                {
                    args.armorAdd += Main.BerzerkersBuffArmor.Value;
                }
                args.armorAdd += Main.BerzerkersArmorAlways.Value * stack;
            }
        }
        // TODO: Ask Moffein for his Pauldron changes and implement them :plead
    }
}
