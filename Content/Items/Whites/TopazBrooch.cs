using MonoMod.Cil;
using System.Threading;

namespace WellRoundedBalance.Items.Whites
{
    internal class TopazBrooch : ItemBase
    {
        public override string Name => ":: Items : Whites : Topaz Brooch";

        public override string InternalPickupToken => "barrierOnKill";

        public override string PickupText => "Gain a temporary barrier on kill.";

        public override string DescText => "Gain a <style=cIsHealing>temporary barrier</style> on kill for <style=cIsHealing>10</style> <style=cStack>(+10 per stack)</style> plus an additional <style=cIsHealing>1.5%</style> of <style=cIsHealing>maximum health</style>.";

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            var attacker = damageReport.attackerBody;
            var victim = damageReport.victim;
            if (attacker && victim)
            {
                var stack = attacker.inventory.GetItemCount(RoR2Content.Items.BarrierOnKill);
                if (stack > 0 && NetworkServer.active && attacker.healthComponent)
                {
                    attacker.healthComponent.AddBarrier(attacker.healthComponent.combinedHealthFraction * 0.015f);
                }
            }
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_healthComponent"),
                    x => x.MatchLdcR4(15f)))
            {
                c.Index += 1;
                c.Next.Operand = 10f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Topaz Brooch Barrier hook");
            }
        }
    }
}