using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    internal class TopazBrooch : ItemBase<TopazBrooch>
    {
        public override string Name => ":: Items : Whites :: Topaz Brooch";

        public override ItemDef InternalPickup => RoR2Content.Items.BarrierOnKill;

        public override string PickupText => "Gain a temporary barrier on kill.";

        public override string DescText => "Gain a <style=cIsHealing>temporary barrier</style> on kill for " +
            StackDesc(flatBarrierGain, flatBarrierGainStack, init => $"<style=cIsHealing>{init}</style>{{Stack}}", noop) +
            StackDesc(percentBarrierGain, percentBarrierGainStack, init => (flatBarrierGain > 0 || flatBarrierGainStack > 0 ? " plus an additional " : "") + $"<style=cIsHealing>{d(init)}</style>{{Stack}} of <style=cIsHealing>maximum health</style>", d) + ".";

        [ConfigField("Base Percent Barrier Gain", "Decimal.", 0.03f)]
        public static float percentBarrierGain;

        [ConfigField("Percent Barrier Gain per Stack", "Decimal.", 0.03f)]
        public static float percentBarrierGainStack;

        [ConfigField("Percent Barrier Gain is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float percentBarrierGainIsHyperbolic;

        [ConfigField("Flat Barrier Gain", 3f)]
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
                        attackerBody.healthComponent.AddBarrier(attackerBody.healthComponent.combinedHealth * StackAmount(percentBarrierGain, percentBarrierGainStack, inventory.GetItemCount(RoR2Content.Items.BarrierOnKill), percentBarrierGainIsHyperbolic));
                }
            }
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);
            int stack = GetItemLoc(c, nameof(RoR2Content.Items.BarrierOnKill));
            if (c.TryGotoNext(x => x.MatchLdloc(stack)) && c.TryGotoNext(x => x.MatchCallOrCallvirt<HealthComponent>(nameof(HealthComponent.AddBarrier))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(flatBarrierGain, flatBarrierGainStack, stack, flatBarrierGainIsHyperbolic));
            }
            else Logger.LogError("Failed to apply Topaz Brooch Barrier hook");
        }
    }
}