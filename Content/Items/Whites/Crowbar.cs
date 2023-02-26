using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class Crowbar : ItemBase
    {
        public override string Name => ":: Items : Whites :: Crowbar";
        public override string InternalPickupToken => "crowbar";

        public override string PickupText => "Deal bonus damage to enemies above " + d(healthThreshold) + " health.";
        public override string DescText => StackDesc(damageIncrease, damageIncreaseStack, 
            init => $"Deal <style=cIsDamage>{d(init)}</style>{{Stack}} damage to enemies above <style=cIsDamage>{d(healthThreshold)} health</style>.",
            stack => d(stack));

        [ConfigField("Damage Increase", "Decimal.", 0.4f)]
        public static float damageIncrease;

        [ConfigField("Damage Increase per Stack", "Decimal.", 0.4f)]
        public static float damageIncreaseStack;

        [ConfigField("Health Threshold", "Decimal.", 0.85f)]
        public static float healthThreshold;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchCallOrCallvirt<HealthComponent>("get_" + nameof(HealthComponent.fullCombinedHealth))) && c.TryGotoNext(x => x.MatchMul()))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_R4, healthThreshold);
            }
            else Main.WRBLogger.LogError("Failed to apply Crowbar Threshold hook");
            if (c.TryGotoNext(x => x.MatchLdloc(19)) && c.TryGotoNext(x => x.MatchStloc(6)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, 19);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(damageIncrease, damageIncreaseStack, stack));
            }
            else Main.WRBLogger.LogError("Failed to apply Crowbar Damage hook");
        }
    }
}