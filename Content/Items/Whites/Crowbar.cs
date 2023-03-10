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

        public override string DescText =>
            StackDesc(damageIncrease, damageIncreaseStack, init => $"Deal <style=cIsDamage>{d(init)}</style>{{Stack}} damage to enemies ", d) +
            StackDesc(healthThreshold, healthThresholdStack, init => $"above <style=cIsDamage>{d(init)}{{Stack}} health</style>", d) + ".";

        [ConfigField("Damage Increase", "Decimal.", 0.4f)]
        public static float damageIncrease;

        [ConfigField("Damage Increase per Stack", "Decimal.", 0.4f)]
        public static float damageIncreaseStack;

        [ConfigField("Damage Increase is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float damageIncreaseIsHyperbolic;

        [ConfigField("Health Threshold", "Decimal.", 0.85f)]
        public static float healthThreshold;

        [ConfigField("Health Threshold per Stack", "Decimal.", 0f)]
        public static float healthThresholdStack;

        [ConfigField("Health Threshold is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float healthThresholdIsHyperbolic;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        public static void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);
            int dmg = -1;
            c.TryGotoNext(x => x.MatchLdfld<DamageInfo>(nameof(DamageInfo.damage)), x => x.MatchStloc(out dmg));
            int stack = GetItemLoc(c, nameof(RoR2Content.Items.Crowbar));
            if (dmg == -1 || stack == -1) return;
            c.Index = 0;
            if (c.TryGotoNext(x => x.MatchCallOrCallvirt<HealthComponent>("get_" + nameof(HealthComponent.fullCombinedHealth))) && c.TryGotoNext(x => x.MatchMul()))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(healthThreshold, healthThresholdStack, stack, healthThresholdIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Crowbar Threshold hook");
            if (c.TryGotoNext(x => x.MatchLdloc(stack)) && c.TryGotoNext(x => x.MatchStloc(dmg)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<int, float>>(stack => 1f + StackAmount(damageIncrease, damageIncreaseStack, stack, damageIncreaseIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Crowbar Damage hook");
        }
    }
}