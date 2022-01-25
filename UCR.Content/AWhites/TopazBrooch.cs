using RoR2;
using MonoMod.Cil;
using UnityEngine.Networking;

namespace UltimateCustomRun
{
    public static class TopazBrooch
    {
        public static void ChangeBarrier(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_healthComponent"),
                x => x.MatchLdcR4(15f)
            );
            c.Index += 1;
            c.Next.Operand = Main.TopazBroochBarrier.Value;
        }
        public static void AddBehavior(DamageReport report)
        {
            if (!NetworkServer.active)
            {
                return;
            }
            CharacterBody att = report.attackerBody;
            Inventory inv = att.inventory;
            if (att && inv)
            {
                var stack = report.attackerBody.inventory.GetItemCount(RoR2Content.Items.BarrierOnKill);
                if (stack > 0)
                {
                    var maxhp = att.healthComponent.fullCombinedHealth;
                    att.healthComponent.AddBarrier(Main.TopazBroochPercentBarrierStack.Value ? maxhp * stack : maxhp);
                }
            }
        }
        // i was told a NetworkServer.active needs to be here :Thonk:
    }
}
