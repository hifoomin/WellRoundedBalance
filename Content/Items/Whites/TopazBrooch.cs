using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    internal class TopazBrooch : ItemBase
    {
        public override string Name => ":: Items : Whites :: Topaz Brooch";

        public override string InternalPickupToken => "barrierOnKill";

        public override string PickupText => "Gain a temporary barrier on kill.";

        public override string DescText => "Gain a <style=cIsHealing>temporary barrier</style> on kill for " +
            StackDesc(flatBarrierGain, flatBarrierGainStack, init => $"<style=cIsHealing>{init}</style>{{Stack}}", noop) +
            StackDesc(percentBarrierGain, percentBarrierGainStack, init => (flatBarrierGain > 0 || flatBarrierGainStack > 0 ? "plus an additional " : "") + $"<style=cIsHealing>{d(init)}</style>{{Stack}} of <style=cIsHealing>maximum health</style>", d) + ".";

        [ConfigField("Percent Barrier Gain", "Decimal.", 0.02f)]
        public static float percentBarrierGain;

        [ConfigField("Percent Barrier Gain per Stack", "Decimal.", 0.02f)]
        public static float percentBarrierGainStack;

        [ConfigField("Percent Barrier Gain is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float percentBarrierGainIsHyperbolic;

        [ConfigField("Flat Barrier Gain", 10f)]
        public static float flatBarrierGain;

        [ConfigField("Flat Barrier Gain per Stack", 0f)]
        public static float flatBarrierGainStack;

        [ConfigField("Flat Barrier Gain is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float flatBarrierGainIsHyperbolic;

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (damageReport.attacker && damageReport.victim)
            {
                var attackerBody = damageReport.attackerBody;
                if (attackerBody)
                {
                    var inventory = attackerBody.inventory;
                    if (inventory && NetworkServer.active && attackerBody.healthComponent)
                        attackerBody.healthComponent.AddBarrier(attackerBody.healthComponent.combinedHealthFraction * StackAmount(percentBarrierGain, percentBarrierGainStack, inventory.GetItemCount(RoR2Content.Items.BarrierOnKill), percentBarrierGainIsHyperbolic));
                }
            }
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchLdloc(49)) && c.TryGotoNext(x => x.MatchCallOrCallvirt<HealthComponent>(nameof(HealthComponent.AddBarrier))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, 49);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(flatBarrierGain, flatBarrierGainStack, stack, flatBarrierGainIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Topaz Brooch Barrier hook");
        }
    }
}