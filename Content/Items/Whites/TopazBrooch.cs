using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    internal class TopazBrooch : ItemBase
    {
        public override string Name => ":: Items : Whites :: Topaz Brooch";

        public override string InternalPickupToken => "barrierOnKill";

        public override string PickupText => "Gain a temporary barrier on kill.";

        public override string DescText => "Gain a <style=cIsHealing>temporary barrier</style> on kill for <style=cIsHealing>" + flatBarrierGain + "</style> <style=cStack>(+" + flatBarrierGain + " per stack)</style>" +
                                           (percentBarrierGain > 0 ? " plus an additional <style=cIsHealing>" + d(percentBarrierGain) + "</style> of <style=cIsHealing>maximum health</style>." : ".");

        [ConfigField("Percent Barrier Gain", "Decimal.", 0.02f)]
        public static float percentBarrierGain;

        [ConfigField("Flat Barrier Gain", "", 10f)]
        public static float flatBarrierGain;

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            var attacker = damageReport.attacker;
            var victim = damageReport.victim;
            if (attacker && victim)
            {
                var attackerBody = damageReport.attackerBody;
                if (attackerBody)
                {
                    var inventory = attackerBody.inventory;
                    if (inventory)
                    {
                        var stack = inventory.GetItemCount(RoR2Content.Items.BarrierOnKill);
                        if (stack > 0 && NetworkServer.active && attackerBody.healthComponent)
                        {
                            attackerBody.healthComponent.AddBarrier(attackerBody.healthComponent.combinedHealthFraction * percentBarrierGain);
                        }
                    }
                }
            }
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt<CharacterBody>("get_healthComponent"),
                    x => x.MatchLdcR4(15f)))
            {
                c.Index += 1;
                c.Next.Operand = flatBarrierGain;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Topaz Brooch Barrier hook");
            }
        }
    }
}