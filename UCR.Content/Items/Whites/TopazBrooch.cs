using RoR2;
using MonoMod.Cil;
using UnityEngine.Networking;

namespace UltimateCustomRun
{
    public class TopazBrooch : Based
    {
        public static float barrier;

        public override string Name => ":: Items : Whites :: Topaz Brooch";
        public override string InternalPickupToken => "barrierOnKill";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Gain a <style=cIsHealing>temporary barrier</style> on kill for <style=cIsHealing>" + barrier + " health <style=cStack>(+" + barrier + " per stack)</style></style>.";
        public override void Init()
        {
            barrier = ConfigOption(15f, "Barrier Gain", "Per Stack. Vanilla is 15");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeBarrier;
            // GlobalEventManager.onCharacterDeathGlobal += TopazBrooch.AddBehavior;
        }
        public static void ChangeBarrier(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_healthComponent"),
                x => x.MatchLdcR4(15f)
            );
            c.Index += 1;
            c.Next.Operand = barrier;
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
        // PLEASE HELP TO FIX
    }
}
