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
        public static void ChangeBuffStats(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before

                // this changes both gorags and pauldron so i will do the stupid workaround instead
            );
        }
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var buff = sender.HasBuff(RoR2Content.Buffs.WarCryBuff);
            var stack = sender.inventory.GetItemCount(RoR2.RoR2Content.Items.WarCryOnMultiKill);
            if (sender.inventory)
            {
                if (stack > 0)
                {
                    if (buff)
                    {
                        args.armorAdd += Main.BerzerkersBuffArmor.Value;
                    }
                }
                args.armorAdd += Main.BerzerkersArmorAlways.Value * stack;
            }
        }

        // this method throws

        // its a fuckin pauldron why does it not add armor lmao

        // TODO: Ask Moffein for his Pauldron changes and implement them :plead
    }
}
